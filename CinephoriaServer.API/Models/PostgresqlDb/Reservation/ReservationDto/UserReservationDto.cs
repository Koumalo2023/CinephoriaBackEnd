using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UserReservationDto
    {
        /// <summary>
        /// Identifiant unique de la réservation.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant effectué la réservation.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Identifiant de la séance réservée.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Prix total de la réservation.
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// Contenu du QR code généré pour la réservation.
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// Indique si le QR code a été validé par un employé.
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// Nom du film
        /// </summary>
        public string MovieName { get; set; } 

        /// <summary>
        /// Nom du cinéma
        /// </summary>
        public string CinemaName { get; set; } 

        /// <summary>
        /// Heure de début de la séance
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Identifiant du film
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Heure de fin de la séance
        /// </summary>
        public DateTime EndTime { get; set; } 

        /// <summary>
        /// Statut de la réservation (par exemple, CONFIRMED ou CANCELLED).
        /// </summary>
        public ReservationStatus Status { get; set; }

        /// <summary>
        /// Nombre de sièges réservés.
        /// </summary>
        public int NumberOfSeats { get; set; }

        /// <summary>
        /// Liste des images déposées sur ce film.
        /// </summary>
        public string PosterUrls { get; set; }

        // <summary>
        /// liste des sièges réservés.
        /// </summary>
        public List<SeatDto> Seats { get; set; }
    }
}