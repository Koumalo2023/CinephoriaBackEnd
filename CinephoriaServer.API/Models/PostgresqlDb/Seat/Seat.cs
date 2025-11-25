using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class Seat : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique du siège.
        /// </summary>
        public int SeatId { get; set; }

        [Required]
        /// <summary>
        /// Identifiant de la salle à laquelle appartient le siège.
        /// </summary>
        public int TheaterId { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Le numéro de siège ne peut pas dépasser 10 caractères.")]
        /// <summary>
        /// Numéro ou identifiant du siège dans la salle (ex: "A1", "B2").
        /// </summary>
        public string SeatNumber { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Indique si le siège est réservé pour les personnes à mobilité réduite.
        /// </summary>
        public bool IsAccessible { get; set; } = false;

        /// <summary>
        /// Indique si le siège est disponible pour réservation.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Navigation vers la salle à laquelle appartient le siège.
        /// </summary>
        public Theater Theater { get; set; }

        /// <summary>
        /// Liste des réservations associées à ce siège.
        /// </summary>
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
