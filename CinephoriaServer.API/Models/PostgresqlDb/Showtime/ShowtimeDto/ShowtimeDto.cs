using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class ShowtimeDto
    {
        /// <summary>
        /// Identifiant unique de la séance.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Identifiant du film projeté durant cette séance.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Identifiant de la salle où a lieu la projection.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Identifiant du cinéma où a lieu la projection.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Heure de début de la séance.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Qualité de la projection (4DX, 3D, etc.).
        /// </summary>
        public ProjectionQuality Quality { get; set; }


        /// <summary>
        /// Heure de fin de la séance.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Prix de la séance en fonction de la qualité.
        /// </summary>
        public decimal Price { get; set; }

        // <summary>
        /// Ajustement de prix (par exemple, -2.00 pour une réduction ou +3.00 pour une majoration).
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Indique si une promotion est appliquée (par exemple, 10% de réduction).
        /// </summary>
        public bool IsPromotion { get; set; }

        /// <summary>
        /// Liste des réservations associées à cette séance.
        /// </summary>
        public ICollection<ReservationDto> Reservations { get; set; } = new List<ReservationDto>();
    }
}
