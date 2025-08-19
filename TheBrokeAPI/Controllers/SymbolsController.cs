using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using TheBrokeClub.API.Options;

namespace TheBrokeClub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SymbolsController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IMemoryCache _cache;
    private readonly AlphaVantageOptions _opt;

    public SymbolsController(IHttpClientFactory httpFactory, IMemoryCache cache, Microsoft.Extensions.Options.IOptions<AlphaVantageOptions> opt)
    {
        _httpFactory = httpFactory;
        _cache = cache;
        _opt = opt.Value;
    }

    // GET /api/symbols/search?q=PETR
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 3)
            return Ok(Array.Empty<object>());

        var term = q.Trim().ToUpperInvariant();
        var cacheKey = $"sym:{term}";
        if (_cache.TryGetValue(cacheKey, out object? cached))
            return Ok(cached);

        var client = _httpFactory.CreateClient();
        var url = $"https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={Uri.EscapeDataString(term)}&apikey={_opt.ApiKey}";

        using var resp = await client.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);

        var list = new List<object>(10);
        if (doc.RootElement.TryGetProperty("bestMatches", out var arr) && arr.ValueKind == JsonValueKind.Array)
        {
            foreach (var m in arr.EnumerateArray())
            {
                var symbol = m.TryGetProperty("1. symbol", out var s) ? s.GetString() : null;
                var name = m.TryGetProperty("2. name", out var n) ? n.GetString() : null;
                var region = m.TryGetProperty("4. region", out var r) ? r.GetString() : null;
                var currency = m.TryGetProperty("8. currency", out var c) ? c.GetString() : null;
                if (!string.IsNullOrWhiteSpace(symbol) && !string.IsNullOrWhiteSpace(name))
                    list.Add(new { symbol, name, region, currency });
                if (list.Count >= 10) break;
            }
        }

        _cache.Set(cacheKey, list, TimeSpan.FromHours(12));
        return Ok(list);
    }
}
