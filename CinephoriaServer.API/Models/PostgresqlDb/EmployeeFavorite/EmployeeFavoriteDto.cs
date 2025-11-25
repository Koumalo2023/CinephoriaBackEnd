using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class EmployeeFavoriteDto
    {
        /// <summary>
        /// Identifiant unique du coup de cœur employé.
        /// </summary>
        public int EmployeeFavoriteId { get; set; }

        /// <summary>
        /// Identifiant de l'employé qui a marqué le film comme coup de cœur.
        /// </summary>
        public string AppUserId { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant du film marqué comme coup de cœur.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Commentaire personnel de l'employé sur le film.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Indique si le film est actuellement marqué comme coup de cœur.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date de création du coup de cœur.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de dernière mise à jour du coup de cœur.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Titre du film (pour l'affichage).
        /// </summary>
        public string MovieTitle { get; set; } = string.Empty;

        /// <summary>
        /// URL de l'affiche du film (pour l'affichage).
        /// </summary>
        public string MoviePosterUrl { get; set; } = string.Empty;
    }

    public class CreateEmployeeFavoriteDto
    {
        [Required]
        /// <summary>
        /// Identifiant du film à marquer comme coup de cœur.
        /// </summary>
        public int MovieId { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères.")]
        /// <summary>
        /// Commentaire personnel de l'employé sur le film.
        /// </summary>
        public string Comment { get; set; } = string.Empty;
    }

    public class UpdateEmployeeFavoriteDto
    {
        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères.")]
        /// <summary>
        /// Nouveau commentaire personnel de l'employé sur le film.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Indique si le film est actuellement marqué comme coup de cœur.
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class EmployeeFavoriteResponseDto
    {
        /// <summary>
        /// Identifiant unique du coup de cœur employé.
        /// </summary>
        public int EmployeeFavoriteId { get; set; }

        /// <summary>
        /// Identifiant de l'employé.
        /// </summary>
        public string AppUserId { get; set; } = string.Empty;

        /// <summary>
        /// Nom de l'employé.
        /// </summary>
        public string EmployeeName { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant du film.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Titre du film.
        /// </summary>
        public string MovieTitle { get; set; } = string.Empty;

        /// <summary>
        /// Commentaire personnel de l'employé sur le film.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Indique si le film est actuellement marqué comme coup de cœur.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Date de création du coup de cœur.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de dernière mise à jour du coup de cœur.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}