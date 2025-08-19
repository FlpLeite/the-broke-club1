using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Infrastructure.Quotes;

public class DbTickerPriceCache : ITickerPriceCache
{
    private readonly AppDbContext _db;
    public DbTickerPriceCache(AppDbContext db) { _db = db; }

    public async Task<(decimal price, DateTime asof, string source)?> TryGetAsync(string ticker, CancellationToken ct)
    {
        var row = await _db.TickerPriceCache.AsNoTracking().FirstOrDefaultAsync(x => x.Ticker == ticker, ct);
        if (row is null) return null;
        return (row.Price, row.AsOf, row.Source);
    }

    public async Task SaveAsync(string ticker, decimal price, DateTime asof, string source, CancellationToken ct)
    {
        var existing = await _db.TickerPriceCache.FirstOrDefaultAsync(x => x.Ticker == ticker, ct);
        if (existing is null)
        {
            _db.TickerPriceCache.Add(new TickerPriceCache { Ticker = ticker, Price = price, AsOf = asof, Source = source });
        }
        else
        {
            existing.Price = price;
            existing.AsOf = asof;
            existing.Source = source;
            _db.TickerPriceCache.Update(existing);
        }
        await _db.SaveChangesAsync(ct);
    }
}
