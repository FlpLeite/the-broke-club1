namespace TheBrokeClub.API.Models;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }

    public ICollection<Transacao>? Transacoes { get; set; }
}
