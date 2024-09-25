namespace CinephoriaBackEnd.Models
{
    public class RoomDto
    {
        /// <summary>
        /// Identifiant unique de la salle.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Identifiant du cinéma associé.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Nombre de sièges dans la salle.
        /// </summary>
        public int NumberOfSeats { get; set; }

        /// <summary>
        /// Qualité de projection de la salle.
        /// </summary>
        public string ProjectionQuality { get; set; }
    }

}
