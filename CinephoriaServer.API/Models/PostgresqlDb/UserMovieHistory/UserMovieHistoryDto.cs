using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UserMovieHistoryDto
    {
        /// <summary>
        /// Identifiant unique de l'entrée d'historique.
        /// </summary>
        public int UserMovieHistoryId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur.
        /// </summary>
        public string AppUserId { get; set; } = string.Empty;

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

        /// <summary>
        /// Titre du film (pour l'affichage).
        /// </summary>
        public string MovieTitle { get; set; } = string.Empty;

        /// <summary>
        /// URL de l'affiche du film (pour l'affichage).
        /// </summary>
        public string MoviePosterUrl { get; set; } = string.Empty;

        /// <summary>
        /// Durée du film (pour l'affichage).
        /// </summary>
        public string MovieDuration { get; set; } = string.Empty;

        /// <summary>
        /// Genre du film (pour l'affichage).
        /// </summary>
        public string MovieGenre { get; set; } = string.Empty;
    }

    public class CreateUserMovieHistoryDto
    {
        [Required]
        /// <summary>
        /// Identifiant du film consulté.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Date et heure de la consultation (optionnel, par défaut DateTime.UtcNow).
        /// </summary>
        public DateTime? LastViewedAt { get; set; }
    }

    public class UpdateUserMovieHistoryDto
    {
        /// <summary>
        /// Date et heure de la dernière consultation.
        /// </summary>
        public DateTime LastViewedAt { get; set; }

        /// <summary>
        /// Nombre de vues à ajouter.
        /// </summary>
        public int ViewCountIncrement { get; set; } = 1;
    }
}