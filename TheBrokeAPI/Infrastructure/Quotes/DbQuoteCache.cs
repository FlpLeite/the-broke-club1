using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Infrastructure.Quotes
{
    public class DbQuoteCache : IQuoteCache
    {
        private readonly AppDbContext _db;
        public DbQuoteCache(AppDbContext db) => _db = db;

        public async Task<(decimal price, DateTime asof)?> TryGetRecentAsync(
            int ativoId, TimeSpan ttl, CancellationToken ct)
        {
            var cutoff = DateTime.UtcNow - ttl;

            var row = await _db.InvestimentoPrecoCache
                .Where(x => x.InvestimentoAtivoId == ativoId && x.AsOf >= cutoff)
                .OrderByDescending(x => x.AsOf)
                .FirstOrDefaultAsync(ct);

            return row is null ? null : (row.Price, row.AsOf);
        }

        public async Task SaveAsync(int ativoId, decimal price, DateTime asof, string source, CancellationToken ct)
        {
            _db.InvestimentoPrecoCache.Add(new InvestimentoPrecoCache
            {
                InvestimentoAtivoId = ativoId,
                Price = price,
                AsOf = asof,
                Source = source
            });
            await _db.SaveChangesAsync(ct);
        }
    }
}
