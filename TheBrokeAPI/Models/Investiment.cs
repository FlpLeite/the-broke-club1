using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TheBrokeClub.API.Models;

[Table("investimentos")]
public class Investimento
{
    [Key]
    [Column("id_investimento")]
    public int IdInvestimento { get; set; }

    [Required]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Required]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Column("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [Column("valor_investido", TypeName = "decimal(18,2)")]
    public decimal ValorInvestido { get; set; }

    [Required]
    [Column("valor_atual", TypeName = "decimal(18,2)")]
    public decimal ValorAtual { get; set; }

    [Required]
    [Column("data_compra")]
    public DateTime DataCompra { get; set; }

    [Required]
    [Column("corretora")]
    public string Corretora { get; set; } = string.Empty;

    [Column("notas")]
    public string? Notas { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    [JsonIgnore]
    public Usuario? Usuario { get; set; }
}