using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class CreateSeatDto
    {
        [Required(ErrorMessage = "L'identifiant de la salle est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant de la salle doit être un nombre positif.")]
        /// <summary>
        /// Identifiant de la salle à laquelle appartient le siège.
        /// </summary>
        public int TheaterId { get; set; }

        [Required(ErrorMessage = "Le numéro du siège est requis.")]
        [StringLength(10, ErrorMessage = "Le numéro du siège ne peut pas dépasser 10 caractères.")]
        /// <summary>
        /// Numéro ou identifiant du siège dans la salle (ex: "A1", "B2").
        /// </summary>
        public string SeatNumber { get; set; }

        [Required(ErrorMessage = "La propriété IsAccessible est requise.")]
        /// <summary>
        /// Indique si le siège est réservé pour les personnes à mobilité réduite.
        /// </summary>
        public bool IsAccessible { get; set; }
    }
}
