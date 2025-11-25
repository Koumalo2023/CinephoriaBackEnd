using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UserMovieHistory : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique de l'entrée d'historique.
        /// </summary>
        public int UserMovieHistoryId { get; set; }

        [Required]
        /// <summary>
        /// Identifiant de l'utilisateur.
        /// </summary>
        public string AppUserId { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Identifiant du film consulté.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Date et heure de la dernière consultation.
        /// </summary>
        public DateTime LastViewedAt { get; set; }

        /// <summary>
        /// Nombre de fois que l'utilisateur a consulté ce film.
        /// </summary>
        public int ViewCount { get; set; } = 1;

        // Navigation properties
        /// <summary>
        /// Utilisateur qui a consulté le film.
        /// </summary>
        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; } = null!;

        /// <summary>
        /// Film consulté.
        /// </summary>
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; } = null!;
    }
}