using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheBrokeClub.API.Options;

namespace TheBrokeClub.API.Infrastructure.Quotes
{
    public class AlphaVantageQuoteProvider : IQuoteProvider
    {
        private readonly HttpClient _http;
        private readonly IQuoteCache _cache;
        private readonly IQuoteLimiter _limiter;
        private readonly AlphaVantageOptions _opt;

        public string Name => "ALPHAVANTAGE";

        public AlphaVantageQuoteProvider(
            HttpClient http,
            IQuoteCache cache,
            IQuoteLimiter limiter,
            IOptions<AlphaVantageOptions> opt)
        {
            _http = http;
            _cache = cache;
            _limiter = limiter;
            _opt = opt.Value;
        }


        public async Task<(decimal price, DateTime asof, bool fromCache, bool isStale)>
        GetQuoteAsync(int ativoId, string ticker, CancellationToken ct)
        {
            var ttl = TimeSpan.FromMinutes(_opt.CacheTtlMinutes);

            var cached = await _cache.TryGetRecentAsync(ativoId, ttl, ct);
            if (cached is not null)
                return (cached.Value.price, cached.Value.asof, true, false);

            if (!await _limiter.TryConsumeAsync(1, ct))
            {
                var stale = await _cache.TryGetRecentAsync(ativoId, TimeSpan.FromDays(30), ct);
                if (stale is not null) return (stale.Value.price, stale.Value.asof, true, true);

                return (0m, DateTime.UtcNow, false, true);
            }

            var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={Uri.EscapeDataString(ticker)}&apikey={_opt.ApiKey}";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            using var resp = await _http.SendAsync(req, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("Note", out _) || root.TryGetProperty("Information", out _))
            {
                var stale = await _cache.TryGetRecentAsync(ativoId, TimeSpan.FromDays(30), ct);
                if (stale is not null) return (stale.Value.price, stale.Value.asof, true, true);
                return (0m, DateTime.UtcNow, false, true);
            }

            if (!root.TryGetProperty("Global Quote", out var quote) || quote.ValueKind != System.Text.Json.JsonValueKind.Object)
            {
                var stale = await _cache.TryGetRecentAsync(ativoId, TimeSpan.FromDays(30), ct);
                if (stale is not null) return (stale.Value.price, stale.Value.asof, true, true);
                return (0m, DateTime.UtcNow, false, true);
            }

            if (!quote.TryGetProperty("05. price", out var priceEl))
            {
                var stale = await _cache.TryGetRecentAsync(ativoId, TimeSpan.FromDays(30), ct);
                if (stale is not null) return (stale.Value.price, stale.Value.asof, true, true);
                return (0m, DateTime.UtcNow, false, true);
            }

            var priceStr = priceEl.GetString();
            if (!decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var price))
            {
                var stale = await _cache.TryGetRecentAsync(ativoId, TimeSpan.FromDays(30), ct);
                if (stale is not null) return (stale.Value.price, stale.Value.asof, true, true);
                return (0m, DateTime.UtcNow, false, true);
            }

            var asof = DateTime.UtcNow;

            await _cache.SaveAsync(ativoId, price, asof, Name, ct);

            return (price, asof, false, false);
        }

    }
}
