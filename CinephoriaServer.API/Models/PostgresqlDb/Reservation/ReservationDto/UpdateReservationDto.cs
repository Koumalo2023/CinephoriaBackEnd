using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UpdateReservationDto
    {
        /// <summary>
        /// Identifiant unique de la réservation à mettre à jour.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Nouveau statut de la réservation : CONFIRMED, CANCELLED.
        /// </summary>
        public ReservationStatus Status { get; set; }

        /// <summary>
        /// Indique si le QR code a été validé par un employé.
        /// </summary>
        public bool IsValidated { get; set; }
    }
}
