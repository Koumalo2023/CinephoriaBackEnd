using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.API.Configurations.EnumConfig;
using CinephoriaServer.API.Configurations.Extensions;
using System.Collections.Generic;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class Movie : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique du film.
        /// </summary>
        public int MovieId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        /// <summary>
        /// Titre du film.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Description du film.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Genre du film.
        /// </summary>
        public MovieGenre Genre { get; set; }

        [Required]
        /// <summary>
        /// Durée du film en minutes.
        /// </summary>
        public string Duration { get; set; }

        [Required]
        /// <summary>
        /// Réalisateur du film.
        /// </summary>
        public List<string> Director { get; set; } = new List<string>();

        [Required]
        /// <summary>
        /// Liste des acteurs principaux du film.
        /// </summary>
        public List<string> Actors { get; set; } = new List<string>();

        [Required]
        /// <summary>
        /// Date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        [Required]
        /// <summary>
        /// Âge minimum recommandé pour visionner ce film.
        /// </summary>
        public MinimumAge MinimumAge { get; set; } = MinimumAge.Public;

        /// <summary>
        /// Label "Coup de cœur" si le film a plu à l'équipe du cinéma.
        /// </summary>
        public bool IsFavorite { get; set; } = false;

        /// <summary>
        /// Moyenne des notes des utilisateurs pour ce film (sur 5).
        /// </summary>
        public double AverageRating { get; set; } = 0.0;

        /// <summary>
        /// Liste des séances associées à ce film (relation un-à-plusieurs).
        /// </summary>
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

        /// <summary>
        /// Liste des images déposées sur ce film.
        /// </summary>
        public string? PosterUrls { get; set; }

        /// <summary>
        /// URL de la bande-annonce du film.
        /// </summary>
        [Url(ErrorMessage = "L'URL de la bande-annonce n'est pas valide.")]
        public string? BandeAnnonce { get; set; }

        /// <summary>
        /// Identifiant unique du film dans The Movie Database (TMDb).
        /// </summary>
        public int? TmdbId { get; set; }

        /// <summary>
        /// Liste des relations de similarité où ce film est la source.
        /// </summary>
        public ICollection<MovieSimilar> SimilarMoviesAsSource { get; set; } = new List<MovieSimilar>();

        /// <summary>
        /// Liste des relations de similarité où ce film est la cible.
        /// </summary>
        public ICollection<MovieSimilar> SimilarMoviesAsTarget { get; set; } = new List<MovieSimilar>();

        /// <summary>
        /// Liste des notations associées à ce film.
        /// </summary>
        public ICollection<MovieRating> MovieRatings { get; set; } = new List<MovieRating>();
    }
}
