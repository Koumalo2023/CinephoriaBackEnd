namespace CinephoriaBackEnd.Models
{
    public class IncidentDto
    {
        /// <summary>
        /// Identifiant unique de l'incident dans MongoDB.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Identifiant de la salle où l'incident s'est produit.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Description de l'incident.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant signalé l'incident.
        /// </summary>
        public string ReporterId { get; set; }

        /// <summary>
        /// Statut actuel de l'incident.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Date de création de l'incident.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de la dernière mise à jour de l'incident.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

}
