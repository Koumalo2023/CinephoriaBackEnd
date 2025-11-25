using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Repository
{
    public interface IUserMovieHistoryRepository
    {
        /// <summary>
        /// Enregistre ou met à jour l'historique de consultation d'un film par un utilisateur.
        /// </summary>
        /// <param name="userMovieHistory">L'objet UserMovieHistory à créer ou mettre à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task AddOrUpdateMovieHistoryAsync(UserMovieHistory userMovieHistory);

        /// <summary>
        /// Enregistre la consultation d'un film par un utilisateur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task RecordMovieViewAsync(string appUserId, int movieId);

        /// <summary>
        /// Récupère les derniers films consultés par un utilisateur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="limit">Le nombre maximum de films à retourner (par défaut: 10).</param>
        /// <returns>Une liste des derniers films consultés.</returns>
        Task<List<UserMovieHistory>> GetRecentMoviesByUserAsync(string appUserId, int limit = 10);

        /// <summary>
        /// Récupère l'entrée d'historique spécifique d'un utilisateur pour un film.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>L'entrée d'historique correspondante, ou null si non trouvée.</returns>
        Task<UserMovieHistory> GetUserMovieHistoryAsync(string appUserId, int movieId);

        /// <summary>
        /// Supprime l'historique de consultation d'un utilisateur pour un film spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteUserMovieHistoryAsync(string appUserId, int movieId);

        /// <summary>
        /// Supprime tout l'historique de consultation d'un utilisateur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task ClearUserMovieHistoryAsync(string appUserId);

        /// <summary>
        /// Récupère l'historique des films consultés par un utilisateur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="limit">Nombre maximum de films à récupérer (optionnel).</param>
        /// <returns>Une liste de films consultés récemment.</returns>
        Task<List<Movie>> GetUserMovieHistoryAsync(string appUserId, int? limit = null);
    }
}