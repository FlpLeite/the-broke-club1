namespace TheBrokeClub.API.Models;

public class TickerPriceCache
{
    public string Ticker { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime AsOf { get; set; }
    public string Source { get; set; } = "ALPHAVANTAGE";
}