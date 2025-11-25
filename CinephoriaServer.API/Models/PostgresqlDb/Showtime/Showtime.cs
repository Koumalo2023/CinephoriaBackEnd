using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class Showtime : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique de la séance.
        /// </summary>
        public int ShowtimeId { get; set; }

        [Required]
        [ForeignKey("Movie")]
        /// <summary>
        /// Identifiant du film projeté durant cette séance.
        /// </summary>
        public int MovieId { get; set; }

        [Required]
        [ForeignKey("Theater")]
        /// <summary>
        /// Identifiant de la salle où a lieu la projection.
        /// </summary>
        public int TheaterId { get; set; }

        [Required]
        /// <summary>
        /// Heure de début de la séance.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Identifiant du cinema où a lieu la projection.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Proprié de navigation vers le Cinema.
        /// </summary>
        public Cinema Cinema { get; set; }

        [Required]
        /// <summary>
        /// Qualité de la projection (4DX, 3D, etc.).
        /// </summary>
        public ProjectionQuality Quality { get; set; }

        /// <summary>
        /// Nombre de sièges disponibles (calculé à partir des sièges réservés).
        /// </summary>
        public int AvailableSeats { get; set; }

        [Required]
        /// <summary>
        /// Heure de fin de la séance.
        /// </summary>
        public DateTime EndTime { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif.")]
        /// <summary>
        /// Prix de la séance en fonction de la qualité.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Ajustement de prix (par exemple, -2.00 pour une réduction ou +3.00 pour une majoration).
        /// </summary>
        public decimal PriceAdjustment { get; set; } = 0.00m;

        /// <summary>
        /// Indique si une promotion est appliquée (par exemple, 10% de réduction).
        /// </summary>
        public bool IsPromotion { get; set; } = false;

        /// <summary>
        /// Statut de la séance (à venir, en cours, terminée, annulée).
        /// </summary>
        public ShowtimeStatus Status { get; set; } = ShowtimeStatus.Upcoming;

        /// <summary>
        /// Heure réelle de début de la séance (peut différer de l'horaire prévu).
        /// </summary>
        public DateTime? ActualStartTime { get; set; }

        /// <summary>
        /// Heure réelle de fin de la séance (peut différer de l'horaire prévu).
        /// </summary>
        public DateTime? ActualEndTime { get; set; }

        /// <summary>
        /// Taux d'occupation de la salle en pourcentage.
        /// </summary>
        public int OccupancyRate { get; set; }

        /// <summary>
        /// Navigation vers le film projeté (relation plusieurs-à-un).
        /// </summary>
        public Movie Movie { get; set; }

        /// <summary>
        /// Navigation vers la salle où se déroule la séance (relation plusieurs-à-un).
        /// </summary>
        public Theater Theater { get; set; }

        /// <summary>
        /// Liste des réservations associées à cette séance.
        /// </summary>
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
