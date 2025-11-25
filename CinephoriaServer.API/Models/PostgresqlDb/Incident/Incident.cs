using static CinephoriaServer.API.Configurations.EnumConfig;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class Incident : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique de l'incident.
        /// </summary>
        public int IncidentId { get; set; }

        [Required]
        /// <summary>
        /// Identifiant de la salle où l'incident a été signalé.
        /// </summary>
        public int TheaterId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Description de l'incident.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        // <summary>
        /// Identifiant de l'employé qui a signalé l'incident.
        /// </summary>
        public string ReportedById { get; set; }

        /// <summary>
        /// Identifiant de l'employé qui a résolu l'incident.
        /// </summary>
        public string ResolvedById { get; set; }



        /// <summary>
        /// Statut de l'incident (PENDING, IN_PROGRESS, RESOLVED).
        /// </summary>
        public IncidentStatus Status { get; set; } = IncidentStatus.Pending;

        /// <summary>
        /// Date à laquelle l'incident a été signalé.
        /// </summary>
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de résolution de l'incident.
        /// </summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>
        /// Les images de l'incident.
        /// </summary>
        public List<string> ImageUrls { get; set; } = new List<string>();

        /// <summary>
        /// Navigation vers l'employé qui a signalé l'incident.
        /// </summary>
        [ForeignKey("ReportedById")]
        public AppUser ReportedBy { get; set; }

        /// <summary>
        /// Navigation vers l'employé qui a resolut l'incident.
        /// </summary>
        [ForeignKey("ResolvedById")]
        public AppUser ResolvedBy { get; set; }

        /// <summary>
        /// Navigation vers la salle où l'incident a été signalé.
        /// </summary>
        [ForeignKey("TheaterId")]
        public Theater Theater { get; set; }
    }
}
