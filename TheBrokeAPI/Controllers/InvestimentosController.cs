using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Application.Services;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestimentosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IInvestimentosService _svc;

    public InvestimentosController(AppDbContext db, IInvestimentosService svc)
    {
        _db = db; _svc = svc;
    }

    public record CreateAtivoRequest(int UsuarioId, string Ticker, string Nome);

    // POST api/investimentos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAtivoRequest body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Ticker) || string.IsNullOrWhiteSpace(body.Nome))
            return BadRequest("Ticker e Nome são obrigatórios.");

        var ativo = new InvestimentoAtivo
        {
            UsuarioId = body.UsuarioId,
            Ticker = body.Ticker.Trim(),
            Nome = body.Nome.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.InvestimentoAtivo.Add(ativo);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetCards), new { usuarioId = body.UsuarioId }, new { ativo.Id });
    }

    // GET api/investimentos/cards?usuarioId=1
    [HttpGet("cards")]
    public async Task<IActionResult> GetCards([FromQuery] int usuarioId, CancellationToken ct)
    {
        var cards = await _svc.GetCardsAsync(usuarioId, ct);
        return Ok(cards);
    }
}
