namespace TheBrokeClub.API.Dtos
{
    public class UpdateTransacaoDto
    {
        public string Tipo { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataTransacao { get; set; }  // aqui é obrigatório
    }
}