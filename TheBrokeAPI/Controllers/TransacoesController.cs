using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Controllers;

[ApiController]
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
    public async Task<ActionResult<Transacao>> CriarTransacao([FromBody] Transacao transacao)
    {
        var usuario = await _context.Usuarios.FindAsync(transacao.IdUsuario);
        if (usuario == null)
        {
            return BadRequest("Usuário não encontrado.");
        }

        transacao.DataTransacao = DateTime.UtcNow;

        _context.Transacoes.Add(transacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTransacoesPorUsuario), new { idUsuario = transacao.IdUsuario }, transacao);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarTransacao(int id, Transacao dto)
    {
        if (id != dto.IdTransacao)
            return BadRequest("Id da URL diferente do body.");

        var existente = await _context.Transacoes.FindAsync(id);
        if (existente == null)
            return NotFound();

        existente.Tipo = dto.Tipo;
        existente.Categoria = dto.Categoria;
        existente.Valor = dto.Valor;
        existente.Descricao = dto.Descricao;
        existente.DataTransacao = dto.DataTransacao;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
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
