
using System;

namespace TheBrokeClub.API.Models;

public class InvestimentoPrecoCache
{
    public int Id { get; set; }
    public int InvestimentoAtivoId { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "BRL";
    public DateTime AsOf { get; set; } 
    public string Source { get; set; } = "ALPHAVANTAGE";
}
