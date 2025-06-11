using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Controllers
{
    public class MetaDto
    {
        public int IdObjetivo { get; set; }
        public int IdUsuario { get; set; }
        public string Titulo { get; set; } = null!;
        public decimal ValorMeta { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime DataLimite { get; set; }
        public int Percentual { get; set; }
    }

    [ApiController]
    [Authorize]
    [Route("metas")]
    public class MetasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MetasController(AppDbContext context)
            => _context = context;

        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<MetaDto>>> GetMetasPorUsuario(int idUsuario)
        {
            var metas = await _context.ObjetivosEconomia
                                     .Where(m => m.IdUsuario == idUsuario)
                                     .ToListAsync();

            var result = metas.Select(m => new MetaDto
            {
                IdObjetivo = m.IdObjetivo,
                IdUsuario = m.IdUsuario,
                Titulo = m.Titulo,
                ValorMeta = m.ValorMeta,
                ValorAtual = m.ValorAtual,
                DataLimite = m.DataLimite,
                Percentual = m.ValorMeta == 0
                   ? 0
                   : (int)Math.Round(m.ValorAtual / m.ValorMeta * 100)
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MetaDto>> CriarMeta([FromBody] Metas nova)
        {
            nova.ValorAtual = 0;
            _context.ObjetivosEconomia.Add(nova);
            await _context.SaveChangesAsync();

            var dto = new MetaDto
            {
                IdObjetivo = nova.IdObjetivo,
                IdUsuario = nova.IdUsuario,
                Titulo = nova.Titulo,
                ValorMeta = nova.ValorMeta,
                ValorAtual = nova.ValorAtual,
                DataLimite = nova.DataLimite,
                Percentual = 0
            };
            return CreatedAtAction(nameof(GetMetasPorUsuario),
                                   new { idUsuario = dto.IdUsuario },
                                   dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarMeta(int id, [FromBody] Metas dto)
        {
            if (id != dto.IdObjetivo)
                return BadRequest("Id da URL diferente do body.");

            var existente = await _context.ObjetivosEconomia.FindAsync(id);
            if (existente == null) return NotFound();

            existente.Titulo = dto.Titulo;
            existente.ValorMeta = dto.ValorMeta;
            existente.DataLimite = dto.DataLimite;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirMeta(int id)
        {
            var meta = await _context.ObjetivosEconomia.FindAsync(id);
            if (meta == null) return NotFound();

            _context.ObjetivosEconomia.Remove(meta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}/progresso")]
        public async Task<IActionResult> AdicionarProgresso(
            int id,
            [FromBody] ProgressoDto dto)
        {
            var meta = await _context.ObjetivosEconomia.FindAsync(id);
            if (meta == null) return NotFound();

            meta.ValorAtual += dto.Valor;
            if (meta.ValorAtual > meta.ValorMeta)
                meta.ValorAtual = meta.ValorMeta;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
