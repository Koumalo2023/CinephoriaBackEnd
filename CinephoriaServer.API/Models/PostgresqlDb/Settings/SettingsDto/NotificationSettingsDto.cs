using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb.Settings
{
    public class NotificationSettingsDto
    {
        [Required]
        public bool EmailNewReservation { get; set; } = true;

        [Required]
        public bool EmailCanceledReservation { get; set; } = true;

        [Required]
        public bool EmailNewUser { get; set; } = true;

        [Required]
        public bool EmailSystemAlerts { get; set; } = true;

        [Required]
        public bool AppNewReservation { get; set; } = true;

        [Required]
        public bool AppCanceledReservation { get; set; } = true;

        [Required]
        public bool AppNewUser { get; set; } = true;

        [Required]
        public bool AppSystemAlerts { get; set; } = true;

        [Required]
        [StringLength(10, ErrorMessage = "La durée de conservation ne peut pas dépasser 10 caractères.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La durée de conservation doit être un nombre.")]
        public string Retention { get; set; } = "30";
    }
}