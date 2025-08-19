using Microsoft.Extensions.Options;
using System.Globalization;

namespace TheBrokeClub.API.Infrastructure.Quotes;

public class B3ScheduleOptions
{
    public string Timezone { get; set; } = "America/Sao_Paulo";
    public string Open { get; set; } = "10:00";
    public string Close { get; set; } = "17:30";
    public int HourlyMinutes { get; set; } = 60;
}

public class QuoteIngestionWorker : BackgroundService
{
    private readonly ILogger<QuoteIngestionWorker> _log;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly B3ScheduleOptions _opt;

    private DateOnly _lastRunDate;
    private bool _didOpenRun;
    private bool _didCloseRun;
    private DateTime _lastHourly = DateTime.MinValue;

    public QuoteIngestionWorker(
        ILogger<QuoteIngestionWorker> log,
        IServiceScopeFactory scopeFactory,
        IOptions<B3ScheduleOptions> opt)
    {
        _log = log;
        _scopeFactory = scopeFactory;
        _opt = opt.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try { await Tick(stoppingToken); }
            catch (OperationCanceledException) { /* shutting down */ }
            catch (Exception ex) { _log.LogError(ex, "QuoteIngestionWorker tick failed"); }
        }
    }

    private async Task Tick(CancellationToken ct)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(_opt.Timezone);
        var nowLocal = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
        var today = DateOnly.FromDateTime(nowLocal);

        if (today != _lastRunDate)
        {
            _lastRunDate = today;
            _didOpenRun = false;
            _didCloseRun = false;
            _lastHourly = DateTime.MinValue;
        }

        TimeSpan open = TimeSpan.ParseExact(_opt.Open, @"hh\:mm", CultureInfo.InvariantCulture);
        TimeSpan close = TimeSpan.ParseExact(_opt.Close, @"hh\:mm", CultureInfo.InvariantCulture);
        var nowTod = nowLocal.TimeOfDay;
        bool inSession = nowTod >= open && nowTod <= close;

        if (!_didOpenRun && Near(nowTod, open, toleranceMinutes: 5))
        {
            await RefreshBatch("OPEN", ct);
            _didOpenRun = true;
            return;
        }

        if (!_didCloseRun && Near(nowTod, close, toleranceMinutes: 5))
        {
            await RefreshBatch("CLOSE", ct);
            _didCloseRun = true;
            return;
        }

        if (inSession && (nowLocal - _lastHourly).TotalMinutes >= _opt.HourlyMinutes)
        {
            await RefreshBatch("HOURLY", ct);
            _lastHourly = nowLocal;
        }
    }

    private static bool Near(TimeSpan t, TimeSpan target, int toleranceMinutes)
        => Math.Abs((t - target).TotalMinutes) <= toleranceMinutes;

    private async Task RefreshBatch(string reason, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var limiter = scope.ServiceProvider.GetRequiredService<IQuoteLimiter>();
        var tracked = scope.ServiceProvider.GetRequiredService<ITrackedTickers>();
        var provider = scope.ServiceProvider.GetRequiredService<IQuoteProvider>();
        var cache = scope.ServiceProvider.GetRequiredService<ITickerPriceCache>();

        var remaining = await limiter.GetRemainingAsync(ct);
        if (remaining <= 0)
        {
            _log.LogInformation("No remaining quote calls. Skip {reason}", reason);
            return;
        }

        var tickers = await tracked.GetTrackedAsync(ct);
        var ordered = tickers
            .OrderByDescending(t => t.popularity)
            .ThenBy(t => t.lastAsOf ?? DateTime.MinValue)
            .ToList();

        int toCall = Math.Min(remaining, ordered.Count);
        if (toCall <= 0)
        {
            _log.LogInformation("No tickers to refresh. Reason={reason}", reason);
            return;
        }

        _log.LogInformation("Refreshing {count} tickers. Reason={reason}", toCall, reason);

        for (int i = 0; i < toCall; i++)
        {
            var tk = ordered[i].ticker;
            try
            {
                // ativoId = 0 porque o provider só precisa do ticker; o debit do limiter já acontece dentro dele
                var (price, asof, _, _) = await provider.GetQuoteAsync(0, tk, ct);
                await cache.SaveAsync(tk, price, asof, "ALPHAVANTAGE", ct);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Failed to refresh {ticker}", tk);
            }
        }
    }
}