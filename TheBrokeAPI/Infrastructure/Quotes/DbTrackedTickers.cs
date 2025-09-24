using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;

namespace TheBrokeClub.API.Infrastructure.Quotes;

public class DbTrackedTickers : ITrackedTickers
{
    private readonly AppDbContext _db;
    public DbTrackedTickers(AppDbContext db) { _db = db; }

    public async Task<IReadOnlyList<(string ticker, int popularity, DateTime? lastAsOf)>> GetTrackedAsync(CancellationToken ct)
    {
        var ativos = await _db.InvestimentoAtivo
            .GroupBy(a => a.Ticker)
            .Select(g => new { ticker = g.Key, popularity = g.Count() })
            .ToListAsync(ct);

        var cache = await _db.TickerPriceCache.AsNoTracking().ToDictionaryAsync(x => x.Ticker, x => (DateTime?)x.AsOf, ct);

        return ativos
            .Select(a => (a.ticker, a.popularity, cache.TryGetValue(a.ticker, out var asof) ? asof : null))
            .ToList();
    }
}
