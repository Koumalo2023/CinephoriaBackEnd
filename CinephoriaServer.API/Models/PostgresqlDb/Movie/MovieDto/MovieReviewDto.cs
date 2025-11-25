using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class MovieReviewDto
    {
        /// <summary>
        /// Identifiant du film.
        /// </summary>
        [Required(ErrorMessage = "L'identifiant du film est requis.")]
        public int MovieId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur.
        /// </summary>
        [Required(ErrorMessage = "L'identifiant de l'utilisateur est requis.")]
        public string AppUserId { get; set; }

        /// <summary>
        /// Note attribuée au film (sur 5).
        /// </summary>
        [Required(ErrorMessage = "La note est requise.")]
        [Range(1, 5, ErrorMessage = "La note doit être comprise entre 1 et 5.")]
        public int Rating { get; set; }

        /// <summary>
        /// Description de l'avis.
        /// </summary>
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string Description { get; set; }
    }
}
