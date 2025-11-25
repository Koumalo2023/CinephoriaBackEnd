using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class CreateReservationDto
    {
        /// <summary>
        /// Identifiant de l'utilisateur faisant la réservation.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Identifiant de la séance réservée.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Liste des sièges réservés.
        /// </summary>
        public ICollection<string> SeatNumbers { get; set; } = new List<string>();

        
    }
}
