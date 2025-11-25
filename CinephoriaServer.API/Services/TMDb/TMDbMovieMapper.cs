using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Models;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Services
{
    public class TMDbMovieMapper
    {
        private const string TMDb_IMAGE_BASE_URL = "https://image.tmdb.org/t/p/original";
        private const int DEFAULT_DURATION = 120;
        private const int MAX_ACTORS = 5;
        private const int MAX_DIRECTORS = 3;

        public Movie MapToMovie(TMDbMovieDetails tmdbMovie)
        {
            if (tmdbMovie == null)
                throw new ArgumentNullException(nameof(tmdbMovie));

            return new Movie
            {
                TmdbId = tmdbMovie.Id,
                Title = tmdbMovie.Title ?? "Titre non disponible",
                Description = string.IsNullOrWhiteSpace(tmdbMovie.Overview) ? "Description non disponible" : tmdbMovie.Overview,
                ReleaseDate = ParseReleaseDate(tmdbMovie.ReleaseDate),
                Duration = FormatDuration(tmdbMovie.Runtime),
                Director = ExtractDirectors(tmdbMovie.Credits?.Crew),
                Actors = ExtractActors(tmdbMovie.Credits?.Cast),
                Genre = MapGenres(tmdbMovie.Genres),
                PosterUrls = BuildPosterUrl(tmdbMovie.PosterPath),
                BandeAnnonce = ExtractTrailerUrl(null), // Initialisé à null, sera mis à jour plus tard
                MinimumAge = DetermineMinimumAge(tmdbMovie),
                AverageRating = NormalizeRating(tmdbMovie.VoteAverage),
                IsFavorite = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private DateTime ParseReleaseDate(string releaseDate)
        {
            if (DateTime.TryParse(releaseDate, out var date))
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            
            // Fallback: utiliser la date actuelle en UTC
            return DateTime.UtcNow;
        }

        private string FormatDuration(int? runtime)
        {
            if (runtime.HasValue && runtime.Value > 0)
                return runtime.Value.ToString();
            
            return DEFAULT_DURATION.ToString();
        }

        private List<string> ExtractDirectors(List<TMDbCrewMember> crew)
        {
            return crew?
                .Where(c => c.Job?.ToLower() == "director")
                .Select(c => c.Name?.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .Take(MAX_DIRECTORS)
                .ToList() ?? new List<string>();
        }

        private List<string> ExtractActors(List<TMDbCastMember> cast)
        {
            return cast?
                .OrderBy(c => c.Order) // TMDb trie par ordre d'importance
                .Select(c => c.Name?.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .Take(MAX_ACTORS)
                .ToList() ?? new List<string>();
        }

        private MovieGenre MapGenres(List<TMDbGenre> tmdbGenres)
        {
            if (tmdbGenres == null || !tmdbGenres.Any())
                return MovieGenre.Documentaire; // Par défaut pour les films sans genre

            var firstGenre = tmdbGenres.First().Name.ToLower();

            return firstGenre switch
            {
                "action" or "adventure" => MovieGenre.Action,
                "aventure" => MovieGenre.Aventure,
                "comedy" => MovieGenre.Comedie,
                "drama" => MovieGenre.Thriller, 
                "horror" => MovieGenre.Horreur,
                "romance" => MovieGenre.Romance,
                "science fiction" or "fantasy" => MovieGenre.Fantastique,
                "thriller" or "mystery" => MovieGenre.Thriller,
                "mystère" => MovieGenre.Mystere,
                "animation" => MovieGenre.Animation,
                "documentary" or "documentaire" => MovieGenre.Documentaire,
                "crime" => MovieGenre.Crime,
                "war" => MovieGenre.Guerre,
                "western" => MovieGenre.Western,
                "family" => MovieGenre.Familiale,
                _ => MovieGenre.Documentaire // Valeur par défaut pour les films documentaires
            };
        }

        private string? BuildPosterUrl(string posterPath)
        {
            if (string.IsNullOrEmpty(posterPath))
                return null;

            return $"{TMDb_IMAGE_BASE_URL}{posterPath}";
        }

        private MinimumAge DetermineMinimumAge(TMDbMovieDetails tmdbMovie)
        {
            // Utiliser le champ 'adult' de TMDbSearchResult si disponible
            // Ou analyser la certification du film
            return MinimumAge.Public; // Temporaire - à améliorer
        }

        private double NormalizeRating(double tmdbRating)
        {
            // TMDb: 0-10, Movie: 0-5
            return Math.Round(tmdbRating / 2, 1);
        }

        /// <summary>
        /// Extrait l'URL de la bande-annonce YouTube à partir de la réponse vidéo TMDb
        /// </summary>
        /// <param name="videoResponse">Réponse des vidéos TMDb</param>
        /// <returns>URL de la bande-annonce YouTube ou null</returns>
        public string? ExtractTrailerUrl(TMDbVideoResponse? videoResponse)
        {
            if (videoResponse?.Results == null || !videoResponse.Results.Any())
                return null;

            // Chercher d'abord une bande-annonce officielle en français
            var trailer = videoResponse.Results
                .FirstOrDefault(v =>
                    v.Type?.ToLower() == "trailer" &&
                    v.Site?.ToLower() == "youtube" &&
                    v.Official &&
                    (v.Language?.ToLower() == "fr" || v.Language?.ToLower() == "fr-fr"));

            // Si pas de bande-annonce officielle en français, prendre la première bande-annonce YouTube
            trailer ??= videoResponse.Results
                .FirstOrDefault(v =>
                    v.Type?.ToLower() == "trailer" &&
                    v.Site?.ToLower() == "youtube");

            // Si toujours pas trouvé, prendre la première vidéo YouTube
            trailer ??= videoResponse.Results
                .FirstOrDefault(v => v.Site?.ToLower() == "youtube");

            return trailer != null ? $"https://www.youtube.com/embed/{trailer.Key}" : null;
        }

        /// <summary>
        /// Met à jour la bande-annonce d'un film avec les données TMDb
        /// </summary>
        /// <param name="movie">Film à mettre à jour</param>
        /// <param name="videoResponse">Réponse des vidéos TMDb</param>
        public void UpdateMovieTrailer(Movie movie, TMDbVideoResponse videoResponse)
        {
            movie.BandeAnnonce = ExtractTrailerUrl(videoResponse);
        }
    }
}