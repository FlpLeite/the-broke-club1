using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Options;

namespace TheBrokeClub.API.Infrastructure.Quotes
{
    public class DbQuoteLimiter : IQuoteLimiter
    {
        private readonly AppDbContext _db;
        private readonly int _dailyLimit;

        public DbQuoteLimiter(AppDbContext db, IOptions<AlphaVantageOptions> opt)
        {
            _db = db;
            _dailyLimit = Math.Max(0, opt.Value.DailyLimit);
        }

        public async Task<bool> TryConsumeAsync(int tokens, CancellationToken ct)
        {
            var day = DateTime.UtcNow.Date;

            if (_dailyLimit <= 0)
                return true; // sem limite => sempre permite

            // retorna 1 quando inseriu/atualizou dentro do limite; 0 quando ultrapassaria e não atualiza
            var affected = await _db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO quote_daily_usage (day, used)
                VALUES ({0}::date, {1})
                ON CONFLICT (day) DO UPDATE
                SET used = quote_daily_usage.used + EXCLUDED.used
                WHERE quote_daily_usage.used + EXCLUDED.used <= {2};
            ", new object[] { day, tokens, _dailyLimit }, ct);

            return affected == 1;
        }

        public async Task<int> GetRemainingAsync(CancellationToken ct)
        {
            var day = DateTime.UtcNow.Date;
            var row = await _db.QuoteDailyUsage.AsNoTracking().FirstOrDefaultAsync(r => r.Day == day, ct);
            return Math.Max(0, _dailyLimit - (row?.Used ?? 0));
        }

        public async Task RefundAsync(int tokens, CancellationToken ct)
        {
            var day = DateTime.UtcNow.Date;

            // garante a existência da linha do dia
            await _db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO quote_daily_usage (day, used)
                VALUES ({0}::date, 0)
                ON CONFLICT (day) DO NOTHING;
            ", new object[] { day }, ct);

            // devolve tokens, sem deixar negativo
            await _db.Database.ExecuteSqlRawAsync(@"
                UPDATE quote_daily_usage
                   SET used = GREATEST(used - {0}, 0)
                 WHERE day = {1}::date;
            ", new object[] { tokens, day }, ct);
        }
    }
}
