using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UserFavorite : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique du favori utilisateur.
        /// </summary>
        public int UserFavoriteId { get; set; }

        [Required]
        /// <summary>
        /// Identifiant de l'utilisateur qui a ajouté le film en favori.
        /// </summary>
        public string AppUserId { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Identifiant du film ajouté en favori.
        /// </summary>
        public int MovieId { get; set; }

        [Required]
        /// <summary>
        /// Date d'ajout du film en favori.
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Utilisateur qui a ajouté le film en favori.
        /// </summary>
        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; } = null!;

        /// <summary>
        /// Film ajouté en favori.
        /// </summary>
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; } = null!;
    }
}