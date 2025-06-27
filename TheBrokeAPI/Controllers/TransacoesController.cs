using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Dtos;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Controllers;

[ApiController]
[Authorize]
[Route("transacoes")]
public class TransacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransacoesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("usuario/{idUsuario}")]
    public async Task<ActionResult<IEnumerable<Transacao>>> GetTransacoesPorUsuario(int idUsuario)
    {
        var transacoes = await _context.Transacoes
            .Where(t => t.IdUsuario == idUsuario)
            .ToListAsync();

        return Ok(transacoes);
    }

    [HttpPost]
    public async Task<ActionResult<Transacao>> CriarTransacao([FromBody] CreateTransacaoDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
            return BadRequest("Usuário não encontrado.");

        var entidade = new Transacao
        {
            IdUsuario = dto.IdUsuario,
            Tipo = dto.Tipo,
            Categoria = dto.Categoria,
            Valor = dto.Valor,
            Descricao = dto.Descricao ?? string.Empty,
            DataTransacao = dto.DataTransacao?.Date
                                ?? DateTime.UtcNow.Date
        };

        _context.Transacoes.Add(entidade);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTransacoesPorUsuario),
            new { idUsuario = entidade.IdUsuario },
            entidade
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarTransacao(
        int id,
        [FromBody] UpdateTransacaoDto dto)
    {
        var existente = await _context.Transacoes.FindAsync(id);
        if (existente == null)
            return NotFound();

        existente.Tipo = dto.Tipo;
        existente.Categoria = dto.Categoria;
        existente.Valor = dto.Valor;
        existente.Descricao = dto.Descricao ?? string.Empty;
        existente.DataTransacao = dto.DataTransacao.Date;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarTransacao(int id)
    {
        var transacao = await _context.Transacoes.FindAsync(id);
        if (transacao == null)
            return NotFound();

        _context.Transacoes.Remove(transacao);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
