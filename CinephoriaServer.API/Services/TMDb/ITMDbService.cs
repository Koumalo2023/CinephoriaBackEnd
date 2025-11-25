using CinephoriaServer.API.Models;

namespace CinephoriaServer.API.Services
{
    public interface ITMDbService
    {
        /// <summary>
        /// Recherche des films dans TMDb par titre
        /// </summary>
        /// <param name="query">Terme de recherche</param>
        /// <param name="page">Numéro de page (optionnel)</param>
        /// <returns>Liste des films correspondants</returns>
        Task<TMDbSearchResult> SearchMoviesAsync(string query, int page = 1);

        /// <summary>
        /// Récupère les détails d'un film TMDb par son identifiant
        /// </summary>
        /// <param name="tmdbId">Identifiant TMDb du film</param>
        /// <returns>Détails complets du film</returns>
        Task<TMDbMovieDetails> GetMovieDetailsAsync(int tmdbId);

        /// <summary>
        /// Récupère les films populaires de TMDb
        /// </summary>
        /// <param name="page">Numéro de page (optionnel)</param>
        /// <returns>Liste des films populaires</returns>
        Task<TMDbSearchResult> GetPopularMoviesAsync(int page = 1);

        /// <summary>
        /// Récupère les films à venir
        /// </summary>
        /// <param name="page">Numéro de page (optionnel)</param>
        /// <returns>Liste des films à venir</returns>
        Task<TMDbSearchResult> GetUpcomingMoviesAsync(int page = 1);

        /// <summary>
        /// Récupère les vidéos (bandes-annonces) d'un film TMDb
        /// </summary>
        /// <param name="tmdbId">Identifiant TMDb du film</param>
        /// <returns>Liste des vidéos du film</returns>
        Task<TMDbVideoResponse> GetMovieVideosAsync(int tmdbId);
    }
}