using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class MovieRating : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique de l'avis.
        /// </summary>
        public int MovieRatingId { get; set; }

        [Required]
        [ForeignKey("Movie")]
        /// <summary>
        /// Référence au film (movies.id).
        /// </summary>
        public int MovieId { get; set; }

        [Required]
        [ForeignKey("AppUser")]
        /// <summary>
        /// Référence à l'utilisateur qui a laissé l'avis.
        /// </summary>
        public string AppUserId { get; set; } = string.Empty;

        [Required]
        [Range(0, 5, ErrorMessage = "La note doit être comprise entre 0 et 5.")]
        /// <summary>
        /// Note donnée par l'utilisateur (sur 5).
        /// </summary>
        public float Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Le commentaire ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Description laissée par l'utilisateur.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Indique si l'avis a été approuvé par un employé.
        /// </summary>
        public bool IsValidated { get; set; } = false;

        /// <summary>
        /// Utilisateur ayant laissé l'avis (navigation).
        /// </summary>
        public AppUser AppUser { get; set; }

        /// <summary>
        /// Film pour lequel l'avis a été laissé (navigation).
        /// </summary>
        public Movie Movie { get; set; }
    }
}
