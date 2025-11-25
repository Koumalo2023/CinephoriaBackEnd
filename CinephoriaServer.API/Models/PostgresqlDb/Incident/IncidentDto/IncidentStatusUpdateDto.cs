using static CinephoriaServer.API.Configurations.EnumConfig;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class IncidentStatusUpdateDto
    {
        /// <summary>
        /// Identifiant unique de l'incident à mettre à jour.
        /// </summary>
        [Required(ErrorMessage = "L'identifiant de l'incident est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant de l'incident doit être un nombre positif.")]
        public int IncidentId { get; set; }

        /// <summary>
        /// Nouveau statut de l'incident (PENDING, IN_PROGRESS, RESOLVED).
        /// </summary>
        [Required(ErrorMessage = "Le statut de l'incident est requis.")]
        public IncidentStatus Status { get; set; }

        /// <summary>
        /// Date de résolution de l'incident.
        /// </summary>
        public DateTime? ResolvedAt { get; set; }
    }
}
