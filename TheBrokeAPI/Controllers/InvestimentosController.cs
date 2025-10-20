using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Dtos;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Controllers;

[ApiController]
[Route("investimentos")]
public class InvestimentosController : ControllerBase
{
    private readonly AppDbContext _context;

    public InvestimentosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("usuario/{idUsuario}")]
    public async Task<ActionResult<IEnumerable<Investimento>>> GetPorUsuario(int idUsuario)
    {
        var investimentos = await _context.Investimentos
            .Where(i => i.IdUsuario == idUsuario)
            .OrderByDescending(i => i.DataCompra)
            .ToListAsync();

        return Ok(investimentos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Investimento>> GetPorId(int id)
    {
        var investimento = await _context.Investimentos.FindAsync(id);
        if (investimento == null)
        {
            return NotFound();
        }

        return investimento;
    }

    [HttpPost]
    public async Task<ActionResult<Investimento>> Criar([FromBody] CreateInvestimentoDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
        {
            return BadRequest("Usuário não encontrado.");
        }

        var investimento = new Investimento
        {
            IdUsuario = dto.IdUsuario,
            Nome = dto.Nome,
            Tipo = dto.Tipo,
            ValorInvestido = dto.ValorInvestido,
            ValorAtual = dto.ValorAtual,
            DataCompra = dto.DataCompra.Date,
            Corretora = dto.Corretora,
            Notas = dto.Notas
        };

        _context.Investimentos.Add(investimento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPorId), new { id = investimento.IdInvestimento }, investimento);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateInvestimentoDto dto)
    {
        var investimento = await _context.Investimentos.FindAsync(id);
        if (investimento == null)
        {
            return NotFound();
        }

        investimento.Nome = dto.Nome;
        investimento.Tipo = dto.Tipo;
        investimento.ValorInvestido = dto.ValorInvestido;
        investimento.ValorAtual = dto.ValorAtual;
        investimento.DataCompra = dto.DataCompra.Date;
        investimento.Corretora = dto.Corretora;
        investimento.Notas = dto.Notas;

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
    public async Task<IActionResult> Remover(int id)
    {
        var investimento = await _context.Investimentos.FindAsync(id);
        if (investimento == null)
        {
            return NotFound();
        }

        _context.Investimentos.Remove(investimento);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}