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

            // 1) Garante que existe a linha do dia (idempotente)
            //    INSERT ... ON CONFLICT DO NOTHING
            await _db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO quote_daily_usage (day, used)
                VALUES ({0}, 0)
                ON CONFLICT (day) DO NOTHING;
            ", new object?[] { day }, ct);

            // 2) Tenta consumir cota apenas se não ultrapassar o limite
            //    UPDATE ... WHERE used + tokens <= _dailyLimit
            var updated = await _db.Database.ExecuteSqlRawAsync(@"
                UPDATE quote_daily_usage
                   SET used = used + {0}
                 WHERE day = {1}
                   AND used + {0} <= {2};
            ", new object?[] { tokens, day, _dailyLimit }, ct);

            // Se 'updated' == 1, consumiu. Se 0, já estava no limite.
            return updated == 1;
        }

        public async Task<int> GetRemainingAsync(CancellationToken ct)
        {
            var day = DateTime.UtcNow.Date;
            var used = await _db.QuoteDailyUsage
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Day == day, ct);

            return Math.Max(0, _dailyLimit - (used?.Used ?? 0));
        }
    }
}
