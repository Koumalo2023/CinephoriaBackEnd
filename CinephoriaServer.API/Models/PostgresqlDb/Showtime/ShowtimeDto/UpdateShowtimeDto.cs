using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UpdateShowtimeDto
    {
        /// <summary>
        /// Identifiant unique de la séance à mettre à jour.
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
        /// Nouvelle heure de début de la séance.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Nouvelle qualité de la projection (4DX, 3D, etc.).
        /// </summary>
        public ProjectionQuality Quality { get; set; }

        /// <summary>
        /// Nouvelle heure de fin de la séance.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Ajustement de prix (par exemple, -2.00 pour une réduction ou +3.00 pour une majoration).
        /// </summary>
        public decimal PriceAdjustment { get; set; } = 0.00m;

        /// <summary>
        /// Indique si une promotion est appliquée (par exemple, 10% de réduction).
        /// </summary>
        public bool IsPromotion { get; set; } = false;
    }
}
