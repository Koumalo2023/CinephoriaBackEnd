using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class CreateIncidentDto
    {
        [Required(ErrorMessage = "L'identifiant de la salle est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant de la salle doit être un nombre positif.")]
        /// <summary>
        /// Identifiant de la salle où l'incident a été signalé.
        /// </summary>
        public int TheaterId { get; set; }

        [Required(ErrorMessage = "La description de l'incident est requise.")]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Description de l'incident.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Identifiant de l'employé qui a signalé l'incident.
        /// </summary>
        public string ReportedBy { get; set; }

        /// <summary>
        /// Liste des images de l'incident.
        /// </summary>
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
