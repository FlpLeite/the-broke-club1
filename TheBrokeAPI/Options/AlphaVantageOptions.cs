namespace TheBrokeClub.API.Options;

public class AlphaVantageOptions
{
    public string ApiKey { get; set; } = "";
    public int CacheTtlMinutes { get; set; } = 15;  // QUOTE_CACHE_TTL_MIN
    public int DailyLimit { get; set; } = 20;       // QUOTE_DAILY_LIMIT (<= 25 no free)
}
