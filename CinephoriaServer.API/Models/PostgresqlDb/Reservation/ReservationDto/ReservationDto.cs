using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class ReservationDto
    {
        /// <summary>
        /// Identifiant unique de la réservation.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant fait la réservation.
        /// </summary>
        public string AppAppUserId { get; set; }

        /// <summary>
        /// Identifiant de la séance réservée.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Liste des sièges réservés.
        /// </summary>
        public ICollection<SeatDto> Seats { get; set; } = new List<SeatDto>();

        /// <summary>
        /// Prix total de la réservation.
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// Contenu du QR code pour validation à l'entrée.
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// Indique si le QR code a été validé par un employé.
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// Statut de la réservation : CONFIRMED, CANCELLED.
        /// </summary>
        public ReservationStatus Status { get; set; }

        /// <summary>
        /// Nombre de sièges réservés.
        /// </summary>
        public int NumberOfSeats { get; set; }
    }
}
