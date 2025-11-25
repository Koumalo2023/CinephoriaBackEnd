using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class LoginUserDto
    {
        /// <summary>
        /// Adresse e-mail de l'utilisateur.
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        public string Email { get; set; }

        /// <summary>
        /// Mot de passe de l'utilisateur.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string Password { get; set; }
    }
}
