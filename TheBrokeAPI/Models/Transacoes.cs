using TheBrokeClub.API.Models;

public class Transacao
{
    public int IdTransacao { get; set; }
    public int IdUsuario { get; set; }

    public string Tipo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataTransacao { get; set; }

    public int? InvestimentoAtivoId { get; set; }

    public decimal? PrecoUnit { get; set; }
    public decimal? Quantidade { get; set; }

    public int? UsuarioIdUsuario { get; set; }
}
