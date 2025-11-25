namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class CancelReservationDto
    {
        /// <summary>
        /// Identifiant unique de la réservation à annuler.
        /// </summary>
        public int ReservationId { get; set; }
    }
}
