using TheBrokeClub.API.Models;

public class Transacao
{
    public int? InvestimentoAtivoId { get; set; }
    public int IdTransacao { get; set; }
    public int IdUsuario { get; set; }
    public string Tipo { get; set; }
    public string Categoria { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; }
    public DateTime DataTransacao { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PrecoUnit { get; set; }

}
