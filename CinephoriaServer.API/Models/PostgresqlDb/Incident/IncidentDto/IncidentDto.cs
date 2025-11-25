using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class IncidentDto
    {
        /// <summary>
        /// Identifiant unique de l'incident.
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        /// Description de l'incident.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Statut de l'incident (PENDING, IN_PROGRESS, RESOLVED).
        /// </summary>
        public IncidentStatus Status { get; set; }

        /// <summary>
        /// Date à laquelle l'incident a été signalé.
        /// </summary>
        public DateTime ReportedAt { get; set; }

        /// <summary>
        /// Date de résolution de l'incident.
        /// </summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>
        /// Identifiant de la salle où l'incident a été signalé.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Nom de la salle où l'incident a été signalé.
        /// </summary>
        public string TheaterName { get; set; }

        /// <summary>
        /// Identifiant de l'employé qui a signalé l'incident.
        /// </summary>
        public string ReportedBy { get; set; }

        /// <summary>
        /// Identifiant de l'employé qui a resolu l'incident.
        /// </summary>
        public string ResolvedBy { get; set; }

        /// <summary>
        /// Liste des images de l'incident.
        /// </summary>
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
