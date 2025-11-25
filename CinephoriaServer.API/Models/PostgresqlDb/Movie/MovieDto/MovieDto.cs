
using static CinephoriaServer.API.Configurations.EnumConfig;
using System.Collections.Generic;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class MovieDto
    {
        /// <summary>
        /// Identifiant unique du film.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Titre du film.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description du film.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Genre du film.
        /// </summary>
        public MovieGenre Genre { get; set; }

        /// <summary>
        /// Durée du film en minutes.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Réalisateur du film.
        /// </summary>
        public List<string> Director { get; set; } = new List<string>();

        /// <summary>
        /// Date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Âge minimum recommandé pour visionner ce film.
        /// </summary>
        public MinimumAge MinimumAge { get; set; }

        /// <summary>
        /// Indique si le film est un "coup de cœur" de l'équipe du cinéma.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Moyenne des notes des utilisateurs pour ce film (sur 5).
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Liste des images associées au film.
        /// </summary>
        public string? PosterUrls { get; set; }

        /// <summary>
        /// URL de la bande-annonce du film.
        /// </summary>
        public string? BandeAnnonce { get; set; }

        /// <summary>
        /// Liste des acteurs du film.
        /// </summary>
        public List<string> Actors { get; set; } = new List<string>();

        /// <summary>
        /// Liste des films similaires.
        /// </summary>
        public ICollection<MovieDto> FilmsSimilaires { get; set; } = new List<MovieDto>();

        /// <summary>
        /// Liste des séances associées à ce film.
        /// </summary>
        public ICollection<ShowtimeDto> Showtimes { get; set; } = new List<ShowtimeDto>();

        /// <summary>
        /// Liste des notations associées à ce film.
        /// </summary>
        public ICollection<MovieRatingDto> MovieRatings { get; set; } = new List<MovieRatingDto>();
    }
}
