using System.Threading;
using System.Threading.Tasks;

namespace TheBrokeClub.API.Infrastructure.Quotes;

public interface ITickerPriceCache
{
    Task<(decimal price, DateTime asof, string source)?> TryGetAsync(string ticker, CancellationToken ct);
    Task SaveAsync(string ticker, decimal price, DateTime asof, string source, CancellationToken ct);
}
