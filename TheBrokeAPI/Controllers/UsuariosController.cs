using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Models;
using BCrypt.Net;
using TheBrokeClub.API.Dtos;


namespace TheBrokeClub.API.Controllers;

[ApiController]
[Route("usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Select(u => new UsuarioResponseDto
            {
                IdUsuario = u.IdUsuario,
                Nome = u.Nome,
                Email = u.Email
            })
            .ToListAsync();

        return usuarios;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(int id)
    {
        var usuario = await _context.Usuarios
            .Where(u => u.IdUsuario == id)
            .Select(u => new UsuarioResponseDto
            {
                IdUsuario = u.IdUsuario,
                Nome = u.Nome,
                Email = u.Email
            })
            .FirstOrDefaultAsync();

        if (usuario == null) return NotFound();
        return usuario;
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponseDto>> CreateUsuario([FromBody] UsuarioCreateDto dto)
    {
        // Validação de e-mail único
        if (_context.Usuarios.Any(u => u.Email == dto.Email))
            return BadRequest("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            DataCriacao = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario },
            new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email
            });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound();

        if (_context.Usuarios.Any(u => u.Email == dto.Email && u.IdUsuario != id))
            return BadRequest("E-mail já cadastrado para outro usuário.");

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound();

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<UsuarioResponseDto>> Login([FromBody] UsuarioLoginDto loginInfo)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == loginInfo.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginInfo.Senha, usuario.SenhaHash))
            return Unauthorized("E-mail ou senha inválidos.");

        return Ok(new UsuarioResponseDto
        {
            IdUsuario = usuario.IdUsuario,
            Nome = usuario.Nome,
            Email = usuario.Email
        });
    }

    [HttpPut("{id}/senha")]
    public async Task<IActionResult> UpdateSenha(int id, [FromBody] UsuarioUpdateSenhaDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(dto.SenhaAtual, usuario.SenhaHash))
            return Unauthorized("Senha atual incorreta.");

        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
        await _context.SaveChangesAsync();

        return NoContent();
    }


}
