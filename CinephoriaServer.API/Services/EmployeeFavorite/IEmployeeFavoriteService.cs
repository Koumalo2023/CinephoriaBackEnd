using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services
{
    public interface IEmployeeFavoriteService
    {
        /// <summary>
        /// Marque un film comme coup de cœur par un employé.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <param name="createDto">Les données de création du coup de cœur.</param>
        /// <returns>L'identifiant du coup de cœur créé.</returns>
        Task<int> CreateEmployeeFavoriteAsync(string appUserId, CreateEmployeeFavoriteDto createDto);

        /// <summary>
        /// Met à jour un coup de cœur employé existant.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur.</param>
        /// <param name="updateDto">Les données de mise à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateEmployeeFavoriteAsync(int employeeFavoriteId, UpdateEmployeeFavoriteDto updateDto);

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
        /// <returns>Le DTO de réponse du coup de cœur.</returns>
        Task<EmployeeFavoriteResponseDto> GetEmployeeFavoriteByIdAsync(int employeeFavoriteId);

        /// <summary>
        /// Récupère tous les coups de cœur d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Une liste de DTO de réponse des coups de cœur.</returns>
        Task<List<EmployeeFavoriteResponseDto>> GetEmployeeFavoritesByUserAsync(string appUserId);

        /// <summary>
        /// Récupère tous les coups de cœur pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de DTO de réponse des coups de cœur.</returns>
        Task<List<EmployeeFavoriteResponseDto>> GetEmployeeFavoritesByMovieAsync(int movieId);

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
        /// <returns>Une liste de DTO de réponse des coups de cœur actifs.</returns>
        Task<List<EmployeeFavoriteResponseDto>> GetAllActiveEmployeeFavoritesAsync();
    }
}