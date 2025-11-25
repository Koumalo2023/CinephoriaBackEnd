using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb.Auth.AppUserDto
{
    public class SubmitMovieReviewDto
    {
        /// <summary>
        /// Identifiant du film pour lequel l'avis est soumis.
        /// </summary>
        [Required]
        public int MovieId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur qui soumet l'avis.
        /// </summary>
        [Required]
        public string AppUserId { get; set; }

        /// <summary>
        /// Note attribuée au film (sur 5).
        /// </summary>
        [Required]
        [Range(0, 5, ErrorMessage = "La note doit être comprise entre 0 et 5.")]
        public int Rating { get; set; }

        /// <summary>
        /// Description de l'avis.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Le commentaire ne peut pas dépasser 1000 caractères.")]
        public string Description { get; set; }
    }
}
