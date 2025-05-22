using System.ComponentModel.DataAnnotations;

namespace TheBrokeClub.API.Dtos
{
    public class UsuarioUpdateSenhaDto
    {
        [Required]
        public string SenhaAtual { get; set; }
        [Required, MinLength(6)]
        public string NovaSenha { get; set; }
    }
}
