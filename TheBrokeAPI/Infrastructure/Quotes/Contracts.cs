using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheBrokeClub.API.Infrastructure.Quotes;

public interface IQuoteCache
{
    Task<(decimal price, DateTime asof)?> TryGetRecentAsync(int ativoId, TimeSpan ttl, CancellationToken ct);
    Task SaveAsync(int ativoId, decimal price, DateTime asof, string source, CancellationToken ct);
}

public interface IQuoteLimiter
{
    Task<bool> TryConsumeAsync(int tokens, CancellationToken ct);
    Task<int> GetRemainingAsync(CancellationToken ct);
}

public interface IQuoteProvider
{
    Task<(decimal price, DateTime asof, bool fromCache, bool isStale)>
        GetQuoteAsync(int ativoId, string ticker, CancellationToken ct);
    string Name { get; }
}
