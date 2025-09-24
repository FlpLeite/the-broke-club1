namespace TheBrokeClub.API.Models
{
    public class InvestimentoAtivo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Ticker { get; set; }
        public string Nome { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
