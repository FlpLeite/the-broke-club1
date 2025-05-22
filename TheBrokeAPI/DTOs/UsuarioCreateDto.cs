using System.ComponentModel.DataAnnotations;

namespace TheBrokeClub.API.Dtos
{
    public class UsuarioCreateDto
    {
        [Required]
        public string Nome { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(6)]
        public string Senha { get; set; }
    }
}
