namespace TheBrokeAPI.Services
{
    public class Quotes
    {
        public interface IQuoteProvider
        {
            Task<(decimal price, DateTime asof, bool fromCache, bool isStale)>
                GetQuoteAsync(int ativoId, string ticker, CancellationToken ct);
            string Name { get; }
        }

        public interface IQuoteCache
        {
            Task<(decimal price, DateTime asof)?> TryGetRecentAsync(int ativoId, TimeSpan ttl, CancellationToken ct);
            Task SaveAsync(int ativoId, decimal price, DateTime asof, string source, CancellationToken ct);
        }

        public interface IQuoteLimiter
        {
            Task<bool> TryConsumeAsync(int tokens, CancellationToken ct); // false => limite atingido
            Task<int> GetRemainingAsync(CancellationToken ct);
        }
    }
}
