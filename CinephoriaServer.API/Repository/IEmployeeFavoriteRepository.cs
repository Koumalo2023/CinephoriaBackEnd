using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Repository
{
    public interface IEmployeeFavoriteRepository
    {
        /// <summary>
        /// Marque un film comme coup de cœur par un employé.
        /// </summary>
        /// <param name="employeeFavorite">L'objet EmployeeFavorite à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateEmployeeFavoriteAsync(EmployeeFavorite employeeFavorite);

        /// <summary>
        /// Met à jour un coup de cœur employé existant.
        /// </summary>
        /// <param name="employeeFavorite">L'objet EmployeeFavorite avec les nouvelles informations.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateEmployeeFavoriteAsync(EmployeeFavorite employeeFavorite);

        /// <summary>
        /// Supprime un coup de cœur employé.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteEmployeeFavoriteAsync(int employeeFavoriteId);

        /// <summary>
        /// Récupère un coup de cœur employé par son identifiant.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur.</param>
        /// <returns>L'objet EmployeeFavorite correspondant.</returns>
        Task<EmployeeFavorite> GetEmployeeFavoriteByIdAsync(int employeeFavoriteId);

        /// <summary>
        /// Récupère tous les coups de cœur d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Une liste de coups de cœur.</returns>
        Task<List<EmployeeFavorite>> GetEmployeeFavoritesByUserAsync(string appUserId);

        /// <summary>
        /// Récupère tous les coups de cœur pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de coups de cœur.</returns>
        Task<List<EmployeeFavorite>> GetEmployeeFavoritesByMovieAsync(int movieId);

        /// <summary>
        /// Vérifie si un employé a déjà marqué un film comme coup de cœur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est déjà marqué comme coup de cœur, sinon false.</returns>
        Task<bool> HasEmployeeFavoriteAsync(string appUserId, int movieId);

        /// <summary>
        /// Récupère tous les coups de cœur actifs.
        /// </summary>
        /// <returns>Une liste de coups de cœur actifs.</returns>
        Task<List<EmployeeFavorite>> GetAllActiveEmployeeFavoritesAsync();
    }
}