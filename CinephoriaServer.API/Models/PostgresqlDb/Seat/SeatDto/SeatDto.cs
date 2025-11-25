namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class SeatDto
    {
        /// <summary>
        /// Identifiant unique du siège.
        /// </summary>
        public int SeatId { get; set; }

        /// <summary>
        /// Identifiant de la salle à laquelle appartient le siège.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Numéro ou identifiant du siège dans la salle (ex: "A1", "B2").
        /// </summary>
        public string SeatNumber { get; set; }

        /// <summary>
        /// Numéro ou identifiant du siège dans la salle (ex: "A1", "B2").
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Indique si le siège est réservé pour les personnes à mobilité réduite.
        /// </summary>
        public bool IsAccessible { get; set; }

        /// <summary>
        /// Indique si le siège est disponible pour réservation.
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
