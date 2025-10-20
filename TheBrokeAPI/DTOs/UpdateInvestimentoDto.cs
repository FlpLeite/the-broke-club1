using System.ComponentModel.DataAnnotations;

namespace TheBrokeClub.API.Dtos;

public class UpdateInvestimentoDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public decimal ValorInvestido { get; set; }

    [Required]
    public decimal ValorAtual { get; set; }

    [Required]
    public DateTime DataCompra { get; set; }

    [Required]
    public string Corretora { get; set; } = string.Empty;

    public string? Notas { get; set; }
}