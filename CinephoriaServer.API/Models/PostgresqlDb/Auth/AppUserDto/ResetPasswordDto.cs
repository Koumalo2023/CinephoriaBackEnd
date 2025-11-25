using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class ResetPasswordDto
    {
        /// <summary>
        /// Token de réinitialisation du mot de passe.
        /// </summary>
        [Required]
        public string Token { get; set; }

        // <summary>
        /// Email de l'utilisateur demandant la réinitialisation du mot de passe.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Nouveau mot de passe de l'utilisateur.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir entre 6 et 100 caractères.")]
        public string NewPassword { get; set; }
    }
}
