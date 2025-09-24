using Microsoft.AspNetCore.Mvc;
using TheBrokeClub.API.Infrastructure.Quotes;

namespace TheBrokeClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestQuoteController : ControllerBase
    {
        private readonly IQuoteProvider _quoteProvider;

        public TestQuoteController(IQuoteProvider quoteProvider)
        {
            _quoteProvider = quoteProvider;
        }

        [HttpGet("{ticker}")]
        public async Task<IActionResult> GetQuote(string ticker, CancellationToken ct)
        {
            var result = await _quoteProvider.GetQuoteAsync(1, ticker, ct);
            return Ok(result);
        }

        [HttpGet("{ativoId:int}/{ticker}")]
        public async Task<IActionResult> GetQuote(int ativoId, string ticker, CancellationToken ct)
        {
            var (price, asof, fromCache, isStale) = await _quoteProvider.GetQuoteAsync(ativoId, ticker, ct);

            return Ok(new
            {
                price,
                asof,
                fromCache,
                isStale
            });
        }
    }
}
