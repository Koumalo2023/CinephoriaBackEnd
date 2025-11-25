using static CinephoriaServer.API.Configurations.EnumConfig;
using System.Collections.Generic;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class MovieDetailsDto
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
        /// Liste des acteurs principaux du film.
        /// </summary>
        public List<string> Actors { get; set; } = new List<string>();

        /// <summary>
        /// Date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Âge minimum recommandé pour visionner ce film.
        /// </summary>
        public MinimumAge MinimumAge { get; set; }

        /// <summary>
        /// Note moyenne du film.
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Liste des URLs des affiches du film.
        /// </summary>
        public string PosterUrls { get; set; }

        /// <summary>
        /// URL de la bande-annonce du film.
        /// </summary>
        public string? BandeAnnonce { get; set; }

        /// <summary>
        /// Liste des films similaires.
        /// </summary>
        public List<MovieDto> FilmsSimilaires { get; set; } = new List<MovieDto>();

        /// <summary>
        /// Liste des séances disponibles pour ce film.
        /// </summary>
        public List<ShowtimeDto> Showtimes { get; set; } = new List<ShowtimeDto>();

        /// <summary>
        /// Liste des avis sur ce film.
        /// </summary>
        public List<MovieRatingDto> Ratings { get; set; } = new List<MovieRatingDto>();
    }

}
