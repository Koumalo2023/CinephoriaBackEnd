using CinephoriaServer.API.Configurations.Extensions;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class TheaterDto 
    {
        /// <summary>
        /// Identifiant unique de la salle de cinéma.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Nom ou numéro de la salle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Capacité maximale de la salle (nombre de sièges).
        /// </summary>
        public int SeatCount { get; set; }

        /// <summary>
        /// Identifiant du cinéma auquel appartient la salle.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Indique si la salle est pleinement fonctionnelle.
        /// </summary>
        public bool IsOperational { get; set; }

        /// <summary>
        /// Qualité de projection disponible dans la salle (ex : "4DX", "3D", "4K").
        /// </summary>
        public ProjectionQuality ProjectionQuality { get; set; }

        /// <summary>
        /// Liste des sièges disponibles dans cette salle.
        /// </summary>
        public ICollection<SeatDto> Seats { get; set; } = new List<SeatDto>();
    }
}
