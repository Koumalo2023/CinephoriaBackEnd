using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb.Settings
{
    public class GeneralSettingsDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Le nom de l'entreprise ne peut pas dépasser 100 caractères.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères.")]
        public string CompanyAddress { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "L'adresse email de contact n'est pas valide.")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required]
        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(10, ErrorMessage = "La langue ne peut pas dépasser 10 caractères.")]
        public string Language { get; set; } = "fr-FR";

        [Required]
        [StringLength(50, ErrorMessage = "Le fuseau horaire ne peut pas dépasser 50 caractères.")]
        public string Timezone { get; set; } = "Europe/Paris";

        [Required]
        [StringLength(20, ErrorMessage = "Le format de date ne peut pas dépasser 20 caractères.")]
        public string DateFormat { get; set; } = "dd/MM/yyyy";

        [Required]
        [StringLength(10, ErrorMessage = "La devise ne peut pas dépasser 10 caractères.")]
        public string Currency { get; set; } = "EUR";
    }
}