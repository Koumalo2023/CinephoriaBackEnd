using CinephoriaServer.API.Models.PostgresqlDb; 
using CinephoriaServer.API.Models; 

namespace CinephoriaServer.API.Services
{
    public interface IMovieService
    {
        /// <summary>
        /// Récupère la liste des derniers films ajoutés.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        Task<List<MovieDto>> GetRecentMoviesAsync();

        /// <summary>
        /// Récupère la liste de tous les films.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        Task<List<MovieDto>> GetAllMoviesAsync();

        /// <summary>
        /// Récupère les détails d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Un objet Movie contenant les détails du film.</returns>
        Task<MovieDetailsDto> GetMovieDetailsAsync(int movieId);


        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        Task<List<ShowtimeDto>> GetMovieSessionsAsync(int movieId);

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="reviewDto">Les données de l'avis.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<string> SubmitMovieReviewAsync(MovieReviewDto reviewDto);

        /// <summary>
        /// Filtre les films en fonction du cinéma, du genre, de la date, de l'année, du réalisateur, des acteurs et de la classification.
        /// </summary>
        /// <param name="filterDto">Les critères de filtrage.</param>
        /// <returns>Une liste de films correspondant aux critères.</returns>
        Task<List<MovieDto>> FilterMoviesAsync(FilterMoviesRequestDto filterDto);

        /// <summary>
        /// Crée un nouveau film.
        /// </summary>
        /// <param name="createMovieDto">Les données du film à créer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<int> CreateMovieAsync(CreateMovieDto createMovieDto);

        /// <summary>
        /// Ajoute une affiche à un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à ajouter.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<string> AddPosterToMovieAsync(int movieId, string posterUrl);

        /// <summary>
        /// Supprime une affiche d'un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<string> RemovePosterFromMovieAsync(int movieId, string posterUrl);

        /// <summary>
        /// Met à jour les informations d'un film existant.
        /// </summary>
        /// <param name="updateMovieDto">Les nouvelles données du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<string> UpdateMovieAsync(UpdateMovieDto updateMovieDto);

        /// <summary>
        /// Supprime un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<string> DeleteMovieAsync(int movieId);

        /// <summary>
        /// Récupère la liste des films qui ont des séances dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films.</returns>
        Task<List<MovieDto>> GetMoviesByCinemaIdAsync(int cinemaId);

        /// <summary>
        /// Enregistre la consultation d'un film par un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<string> RecordMovieViewAsync(string userId, int movieId);

        /// <summary>
        /// Récupère l'historique des films consultés par un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="limit">Nombre maximum de films à récupérer (optionnel).</param>
        /// <returns>Une liste de films consultés récemment.</returns>
        Task<List<MovieDto>> GetUserMovieHistoryAsync(string userId, int? limit = null);

        /// <summary>
        /// Récupère tous les films qui ont au moins une séance programmée.
        /// </summary>
        /// <returns>Une liste de films avec séances.</returns>
        Task<List<MovieDto>> GetAllMoviesWithShowtimeAsync();

        /// <summary>
        /// Recherche des films dans TMDb par titre.
        /// </summary>
        /// <param name="searchRequest">Requête de recherche TMDb.</param>
        /// <returns>Résultats de la recherche TMDb.</returns>
        Task<TMDbSearchResult> SearchTMDbMoviesAsync(TMDbSearchRequestDto searchRequest);

        /// <summary>
        /// Importe un film depuis TMDb dans la base locale.
        /// </summary>
        /// <param name="importRequest">Requête d'import TMDb.</param>
        /// <returns>Identifiant du film importé.</returns>
        Task<int> ImportMovieFromTMDbAsync(TMDbImportRequestDto importRequest);
    }

}
