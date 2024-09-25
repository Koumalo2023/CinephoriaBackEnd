namespace CinephoriaBackEnd.Models
{
    public class ShowtimeDto
    {
        /// <summary>
        /// Identifiant unique de la séance.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Identifiant du film projeté lors de la séance.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Identifiant de la salle où a lieu la séance.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Heure de début de la séance.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Heure de fin de la séance.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Qualité de la projection.
        /// </summary>
        public string Quality { get; set; }

        /// <summary>
        /// Nombre de sièges disponibles.
        /// </summary>
        public int AvailableSeats { get; set; }

        /// <summary>
        /// Prix de la séance.
        /// </summary>
        public float Price { get; set; }
    }

}
