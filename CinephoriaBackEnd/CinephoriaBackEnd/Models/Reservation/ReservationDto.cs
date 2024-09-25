namespace CinephoriaBackEnd.Models
{
    public class ReservationDto
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
        /// Numéro du siège réservé.
        /// </summary>
        public int SeatNumber { get; set; }

        /// <summary>
        /// Prix de la réservation.
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// QR code associé à la réservation.
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// Statut de la réservation.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Date de création de la réservation.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de la dernière mise à jour de la réservation.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

}
