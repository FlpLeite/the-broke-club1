using Microsoft.AspNetCore.Mvc;
using TheBrokeClub.API.Infrastructure.Quotes;

namespace TheBrokeClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteDebugController : ControllerBase
    {
        private readonly IQuoteLimiter _limiter;
        public QuoteDebugController(IQuoteLimiter limiter) { _limiter = limiter; }

        [HttpGet("remaining")]
        public async Task<IActionResult> Remaining(CancellationToken ct)
            => Ok(new { remaining = await _limiter.GetRemainingAsync(ct) });
    }
}
