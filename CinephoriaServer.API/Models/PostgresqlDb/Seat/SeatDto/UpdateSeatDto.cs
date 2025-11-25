using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UpdateSeatDto
    {
        /// <summary>
        /// Identifiant unique du siège.
        /// </summary>
        public int SeatId { get; set; }

        /// <summary>
        /// Numéro ou identifiant du siège dans la salle (ex: "A1", "B2").
        /// </summary>
        [Required]
        [StringLength(10, ErrorMessage = "Le numéro de siège ne peut pas dépasser 10 caractères.")]
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>
        /// Indique si le siège est réservé pour les personnes à mobilité réduite.
        /// </summary>
        [Required]
        public bool IsAccessible { get; set; }

        /// <summary>
        /// Indique si le siège est disponible pour réservation.
        /// </summary>
        [Required]
        public bool IsAvailable { get; set; }
    }

}
