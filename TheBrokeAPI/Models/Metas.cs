using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TheBrokeClub.API.Models
{
    [Table("metas")]
    public class Metas
    {
        [Key]
        [Column("id_objetivo")]
        public int IdObjetivo { get; set; }

        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("titulo")]
        public string Titulo { get; set; }

        [Required]
        [Column("valor_meta", TypeName = "decimal(18,2)")]
        public decimal ValorMeta { get; set; }

        [Required]
        [Column("valor_atual", TypeName = "decimal(18,2)")]
        public decimal ValorAtual { get; set; }

        [Required]
        [Column("data_limite")]
        public DateTime DataLimite { get; set; }

        [Required]
        [Column("categoria")]
        public string Categoria { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
