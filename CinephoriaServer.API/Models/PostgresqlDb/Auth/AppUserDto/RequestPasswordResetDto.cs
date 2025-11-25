using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb.Auth.AppUserDto
{
    public class RequestPasswordResetDto
    {
        /// <summary>
        /// Adresse e-mail de l'utilisateur.
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        public string Email { get; set; }
    }
}
