using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UpdateIncidentDto
    {
        [Required(ErrorMessage = "L'identifiant de l'incident est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant de l'incident doit être un nombre positif.")]
        /// <summary>
        /// Identifiant unique de l'incident à mettre à jour.
        /// </summary>
        public int IncidentId { get; set; }

        [Required(ErrorMessage = "Le statut de l'incident est requis.")]
        /// <summary>
        /// Nouveau statut de l'incident (PENDING, IN_PROGRESS, RESOLVED).
        /// </summary>
        public IncidentStatus Status { get; set; }

        /// <summary>
        /// Date de résolution de l'incident.
        /// </summary>
        public DateTime? ResolvedAt { get; set; }

        [Required(ErrorMessage = "La description de l'incident est requise.")]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Description de l'incident.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Liste des images de l'incident.
        /// </summary>
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
