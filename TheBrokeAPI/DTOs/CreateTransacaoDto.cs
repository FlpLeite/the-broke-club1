namespace TheBrokeClub.API.Dtos
{
    public class CreateTransacaoDto
    {
        public int IdUsuario { get; set; }
        public string Tipo { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataTransacao { get; set; }
    }
}