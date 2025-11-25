using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class EmployeeFavorite : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique du coup de cœur employé.
        /// </summary>
        public int EmployeeFavoriteId { get; set; }

        [Required]
        /// <summary>
        /// Identifiant de l'employé qui a marqué le film comme coup de cœur.
        /// </summary>
        public string AppUserId { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Identifiant du film marqué comme coup de cœur.
        /// </summary>
        public int MovieId { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères.")]
        /// <summary>
        /// Commentaire personnel de l'employé sur le film.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Indique si le film est actuellement marqué comme coup de cœur.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Employé qui a marqué le film comme coup de cœur.
        /// </summary>
        public virtual AppUser AppUser { get; set; } = null!;

        /// <summary>
        /// Film marqué comme coup de cœur.
        /// </summary>
        public virtual Movie Movie { get; set; } = null!;
    }
}