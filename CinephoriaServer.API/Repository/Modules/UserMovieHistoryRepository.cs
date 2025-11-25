using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public class UserMovieHistoryRepository : IUserMovieHistoryRepository
    {
        private readonly CinephoriaDbContext _context;

        public UserMovieHistoryRepository(CinephoriaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Enregistre ou met à jour l'historique de consultation d'un film par un utilisateur.
        /// </summary>
        /// <param name="userMovieHistory">L'objet UserMovieHistory à créer ou mettre à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task AddOrUpdateMovieHistoryAsync(UserMovieHistory userMovieHistory)
        {
            var existingHistory = await _context.Set<UserMovieHistory>()
                .FirstOrDefaultAsync(umh => umh.AppUserId == userMovieHistory.AppUserId && umh.MovieId == userMovieHistory.MovieId);

            if (existingHistory != null)
            {
                // Mise à jour de l'entrée existante
                existingHistory.LastViewedAt = DateTime.UtcNow;
                existingHistory.ViewCount++;
                existingHistory.UpdatedAt = DateTime.UtcNow;
                
                _context.Set<UserMovieHistory>().Update(existingHistory);
            }
            else
            {
                // Création d'une nouvelle entrée
                userMovieHistory.CreatedAt = DateTime.UtcNow;
                userMovieHistory.UpdatedAt = DateTime.UtcNow;
                userMovieHistory.LastViewedAt = DateTime.UtcNow;
                
                _context.Set<UserMovieHistory>().Add(userMovieHistory);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Récupère les derniers films consultés par un utilisateur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="limit">Le nombre maximum de films à retourner (par défaut: 10).</param>
        /// <returns>Une liste des derniers films consultés.</returns>
        public async Task<List<UserMovieHistory>> GetRecentMoviesByUserAsync(string appUserId, int limit = 10)
        {
            return await _context.Set<UserMovieHistory>()
                .Include(umh => umh.Movie)
                .Where(umh => umh.AppUserId == appUserId)
                .OrderByDescending(umh => umh.LastViewedAt)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère l'entrée d'historique spécifique d'un utilisateur pour un film.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>L'entrée d'historique correspondante, ou null si non trouvée.</returns>
        public async Task<UserMovieHistory> GetUserMovieHistoryAsync(string appUserId, int movieId)
        {
            return await _context.Set<UserMovieHistory>()
                .Include(umh => umh.Movie)
                .FirstOrDefaultAsync(umh => umh.AppUserId == appUserId && umh.MovieId == movieId);
        }

        /// <summary>
        /// Supprime l'historique de consultation d'un utilisateur pour un film spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteUserMovieHistoryAsync(string appUserId, int movieId)
        {
            var history = await _context.Set<UserMovieHistory>()
                .FirstOrDefaultAsync(umh => umh.AppUserId == appUserId && umh.MovieId == movieId);

            if (history != null)
            {
                _context.Set<UserMovieHistory>().Remove(history);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Supprime tout l'historique de consultation d'un utilisateur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task ClearUserMovieHistoryAsync(string appUserId)
        {
            var userHistory = await _context.Set<UserMovieHistory>()
                .Where(umh => umh.AppUserId == appUserId)
                .ToListAsync();

            if (userHistory.Any())
            {
                _context.Set<UserMovieHistory>().RemoveRange(userHistory);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Enregistre la consultation d'un film par un utilisateur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task RecordMovieViewAsync(string appUserId, int movieId)
        {
            var userMovieHistory = new UserMovieHistory
            {
                AppUserId = appUserId,
                MovieId = movieId,
                ViewCount = 1,
                LastViewedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await AddOrUpdateMovieHistoryAsync(userMovieHistory);
        }

        /// <summary>
        /// Récupère l'historique des films consultés par un utilisateur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="limit">Nombre maximum de films à récupérer (optionnel).</param>
        /// <returns>Une liste de films consultés récemment.</returns>
        public async Task<List<Movie>> GetUserMovieHistoryAsync(string appUserId, int? limit = null)
        {
            var userMovieHistories = await GetRecentMoviesByUserAsync(appUserId, limit ?? 10);
            return userMovieHistories.Select(umh => umh.Movie).ToList();
        }
    }
}