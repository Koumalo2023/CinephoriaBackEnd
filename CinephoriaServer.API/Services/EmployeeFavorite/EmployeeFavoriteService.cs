using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using Microsoft.EntityFrameworkCore;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Services
{
    public class EmployeeFavoriteService : IEmployeeFavoriteService
    {
        private readonly IEmployeeFavoriteRepository _employeeFavoriteRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMovieRepository _movieRepository;

        public EmployeeFavoriteService(
            IEmployeeFavoriteRepository employeeFavoriteRepository,
            IUserRepository userRepository,
            IMovieRepository movieRepository)
        {
            _employeeFavoriteRepository = employeeFavoriteRepository;
            _userRepository = userRepository;
            _movieRepository = movieRepository;
        }

        /// <summary>
        /// Marque un film comme coup de cœur par un employé.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <param name="createDto">Les données de création du coup de cœur.</param>
        /// <returns>L'identifiant du coup de cœur créé.</returns>
        public async Task<int> CreateEmployeeFavoriteAsync(string appUserId, CreateEmployeeFavoriteDto createDto)
        {
            // Vérifier si l'utilisateur existe et est un employé
            var user = await _userRepository.GetByIdAsync(appUserId);
            if (user == null)
                throw new NotFoundException("Utilisateur non trouvé.");

            if (user.Role != UserRole.Employee && user.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("Seuls les employés et administrateurs peuvent marquer des films comme coups de cœur.");

            // Vérifier si le film existe
            var movie = await _movieRepository.GetByIdAsync(createDto.MovieId);
            if (movie == null)
                throw new NotFoundException("Film non trouvé.");

            // Vérifier si l'employé a déjà marqué ce film comme coup de cœur
            var hasFavorite = await _employeeFavoriteRepository.HasEmployeeFavoriteAsync(appUserId, createDto.MovieId);
            if (hasFavorite)
                throw new InvalidOperationException("Vous avez déjà marqué ce film comme coup de cœur.");

            var employeeFavorite = new EmployeeFavorite
            {
                AppUserId = appUserId,
                MovieId = createDto.MovieId,
                Comment = createDto.Comment,
                IsActive = true
            };

            await _employeeFavoriteRepository.CreateEmployeeFavoriteAsync(employeeFavorite);
            return employeeFavorite.EmployeeFavoriteId;
        }

        /// <summary>
        /// Met à jour un coup de cœur employé existant.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur.</param>
        /// <param name="updateDto">Les données de mise à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateEmployeeFavoriteAsync(int employeeFavoriteId, UpdateEmployeeFavoriteDto updateDto)
        {
            var employeeFavorite = await _employeeFavoriteRepository.GetEmployeeFavoriteByIdAsync(employeeFavoriteId);
            if (employeeFavorite == null)
                throw new NotFoundException("Coup de cœur non trouvé.");

            if (!string.IsNullOrEmpty(updateDto.Comment))
                employeeFavorite.Comment = updateDto.Comment;

            if (updateDto.IsActive.HasValue)
                employeeFavorite.IsActive = updateDto.IsActive.Value;

            await _employeeFavoriteRepository.UpdateEmployeeFavoriteAsync(employeeFavorite);
        }

        /// <summary>
        /// Supprime un coup de cœur employé.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteEmployeeFavoriteAsync(int employeeFavoriteId)
        {
            var employeeFavorite = await _employeeFavoriteRepository.GetEmployeeFavoriteByIdAsync(employeeFavoriteId);
            if (employeeFavorite == null)
                throw new NotFoundException("Coup de cœur non trouvé.");

            await _employeeFavoriteRepository.DeleteEmployeeFavoriteAsync(employeeFavoriteId);
        }

        /// <summary>
        /// Récupère un coup de cœur employé par son identifiant.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur.</param>
        /// <returns>Le DTO de réponse du coup de cœur.</returns>
        public async Task<EmployeeFavoriteResponseDto> GetEmployeeFavoriteByIdAsync(int employeeFavoriteId)
        {
            var employeeFavorite = await _employeeFavoriteRepository.GetEmployeeFavoriteByIdAsync(employeeFavoriteId);
            if (employeeFavorite == null)
                throw new NotFoundException("Coup de cœur non trouvé.");

            return MapToResponseDto(employeeFavorite);
        }

        /// <summary>
        /// Récupère tous les coups de cœur d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Une liste de DTO de réponse des coups de cœur.</returns>
        public async Task<List<EmployeeFavoriteResponseDto>> GetEmployeeFavoritesByUserAsync(string appUserId)
        {
            var employeeFavorites = await _employeeFavoriteRepository.GetEmployeeFavoritesByUserAsync(appUserId);
            return employeeFavorites.Select(MapToResponseDto).ToList();
        }

        /// <summary>
        /// Récupère tous les coups de cœur pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de DTO de réponse des coups de cœur.</returns>
        public async Task<List<EmployeeFavoriteResponseDto>> GetEmployeeFavoritesByMovieAsync(int movieId)
        {
            var employeeFavorites = await _employeeFavoriteRepository.GetEmployeeFavoritesByMovieAsync(movieId);
            return employeeFavorites.Select(MapToResponseDto).ToList();
        }

        /// <summary>
        /// Vérifie si un employé a déjà marqué un film comme coup de cœur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est déjà marqué comme coup de cœur, sinon false.</returns>
        public async Task<bool> HasEmployeeFavoriteAsync(string appUserId, int movieId)
        {
            return await _employeeFavoriteRepository.HasEmployeeFavoriteAsync(appUserId, movieId);
        }

        /// <summary>
        /// Récupère tous les coups de cœur actifs.
        /// </summary>
        /// <returns>Une liste de DTO de réponse des coups de cœur actifs.</returns>
        public async Task<List<EmployeeFavoriteResponseDto>> GetAllActiveEmployeeFavoritesAsync()
        {
            var employeeFavorites = await _employeeFavoriteRepository.GetAllActiveEmployeeFavoritesAsync();
            return employeeFavorites.Select(MapToResponseDto).ToList();
        }

        /// <summary>
        /// Map un objet EmployeeFavorite vers EmployeeFavoriteResponseDto.
        /// </summary>
        /// <param name="employeeFavorite">L'objet EmployeeFavorite à mapper.</param>
        /// <returns>Le DTO de réponse correspondant.</returns>
        private EmployeeFavoriteResponseDto MapToResponseDto(EmployeeFavorite employeeFavorite)
        {
            return new EmployeeFavoriteResponseDto
            {
                EmployeeFavoriteId = employeeFavorite.EmployeeFavoriteId,
                AppUserId = employeeFavorite.AppUserId,
                EmployeeName = $"{employeeFavorite.AppUser.FirstName} {employeeFavorite.AppUser.LastName}",
                MovieId = employeeFavorite.MovieId,
                MovieTitle = employeeFavorite.Movie.Title,
                Comment = employeeFavorite.Comment,
                IsActive = employeeFavorite.IsActive,
                CreatedAt = employeeFavorite.CreatedAt,
                UpdatedAt = employeeFavorite.UpdatedAt
            };
        }
    }
}