using System.ComponentModel.DataAnnotations;

namespace TheBrokeClub.API.Dtos
{
    public class UsuarioUpdateDto
    {
        [Required]
        public string Nome { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
