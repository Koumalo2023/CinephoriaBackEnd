using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb.Settings
{
    public class SecuritySettingsDto
    {
        [Required]
        [Range(6, 20, ErrorMessage = "La longueur minimale du mot de passe doit être entre 6 et 20 caractères.")]
        public int PasswordMinLength { get; set; } = 8;

        [Required]
        public bool PasswordRequireUppercase { get; set; } = true;

        [Required]
        public bool PasswordRequireNumber { get; set; } = true;

        [Required]
        public bool PasswordRequireSpecial { get; set; } = true;

        [Required]
        [StringLength(10, ErrorMessage = "L'expiration du mot de passe ne peut pas dépasser 10 caractères.")]
        [RegularExpression(@"^\d+|never$", ErrorMessage = "L'expiration doit être un nombre ou 'never'.")]
        public string PasswordExpiry { get; set; } = "90";
    }
}