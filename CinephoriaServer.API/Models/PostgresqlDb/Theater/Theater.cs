using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class Theater : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique de la salle de projection.
        /// </summary>
        public int TheaterId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Le nom de la salle ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Nom ou numéro de la salle.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Capacité maximale de la salle (nombre de sièges).
        /// </summary>
        public int SeatCount { get; set; }

        [Required]
        [ForeignKey("Cinema")]
        /// <summary>
        /// Identifiant du cinéma auquel appartient la salle.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Indique si la salle est pleinement fonctionnelle.
        /// </summary>
        public bool IsOperational { get; set; } = true;

        [Required]
        /// <summary>
        /// Qualité de projection disponible dans la salle (ex : "4DX", "3D", "4K").
        /// </summary>
        public ProjectionQuality ProjectionQuality { get; set; }

        /// <summary>
        /// Liste des incidents signalés dans la salle (relation un-à-plusieurs).
        /// </summary>
        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();

        /// <summary>
        /// Liste des séances projetées dans cette salle.
        /// </summary>
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

        /// <summary>
        /// Liste des sièges disponibles dans cette salle.
        /// </summary>
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        /// <summary>
        /// Navigation vers le cinéma auquel la salle appartient (relation plusieurs-à-un).
        /// </summary>
        public Cinema Cinema { get; set; }
    }
}
