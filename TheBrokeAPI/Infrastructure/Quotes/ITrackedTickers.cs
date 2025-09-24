namespace TheBrokeClub.API.Infrastructure.Quotes;

public interface ITrackedTickers
{
    Task<IReadOnlyList<(string ticker, int popularity, DateTime? lastAsOf)>> GetTrackedAsync(CancellationToken ct);
}
