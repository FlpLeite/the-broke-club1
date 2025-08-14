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

        // GET api/testquote/PETR4.SAO
        [HttpGet("{ticker}")]
        public async Task<IActionResult> GetQuote(string ticker, CancellationToken ct)
        {
            // Simulando ativo de ID fixo = 1 só para teste
            var result = await _quoteProvider.GetQuoteAsync(1, ticker, ct);
            return Ok(result);
        }
    }
}
