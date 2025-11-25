using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public class EmployeeFavoriteRepository : IEmployeeFavoriteRepository
    {
        private readonly CinephoriaDbContext _context;

        public EmployeeFavoriteRepository(CinephoriaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Marque un film comme coup de cœur par un employé.
        /// </summary>
        /// <param name="employeeFavorite">L'objet EmployeeFavorite à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CreateEmployeeFavoriteAsync(EmployeeFavorite employeeFavorite)
        {
            employeeFavorite.CreatedAt = DateTime.UtcNow;
            employeeFavorite.UpdatedAt = DateTime.UtcNow;

            _context.Set<EmployeeFavorite>().Add(employeeFavorite);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour un coup de cœur employé existant.
        /// </summary>
        /// <param name="employeeFavorite">L'objet EmployeeFavorite avec les nouvelles informations.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateEmployeeFavoriteAsync(EmployeeFavorite employeeFavorite)
        {
            employeeFavorite.UpdatedAt = DateTime.UtcNow;

            _context.Set<EmployeeFavorite>().Update(employeeFavorite);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime un coup de cœur employé.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur à supprimer.</param>
        /// <returns>Une tâche asynchrene.</returns>
        public async Task DeleteEmployeeFavoriteAsync(int employeeFavoriteId)
        {
            var employeeFavorite = await _context.Set<EmployeeFavorite>()
                .FindAsync(employeeFavoriteId);

            if (employeeFavorite != null)
            {
                _context.Set<EmployeeFavorite>().Remove(employeeFavorite);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Récupère un coup de cœur employé par son identifiant.
        /// </summary>
        /// <param name="employeeFavoriteId">L'identifiant du coup de cœur.</param>
        /// <returns>L'objet EmployeeFavorite correspondant.</returns>
        public async Task<EmployeeFavorite> GetEmployeeFavoriteByIdAsync(int employeeFavoriteId)
        {
            return await _context.Set<EmployeeFavorite>()
                .Include(ef => ef.AppUser)
                .Include(ef => ef.Movie)
                .FirstOrDefaultAsync(ef => ef.EmployeeFavoriteId == employeeFavoriteId);
        }

        /// <summary>
        /// Récupère tous les coups de cœur d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Une liste de coups de cœur.</returns>
        public async Task<List<EmployeeFavorite>> GetEmployeeFavoritesByUserAsync(string appUserId)
        {
            return await _context.Set<EmployeeFavorite>()
                .Include(ef => ef.AppUser)
                .Include(ef => ef.Movie)
                .Where(ef => ef.AppUserId == appUserId)
                .OrderByDescending(ef => ef.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère tous les coups de cœur pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de coups de cœur.</returns>
        public async Task<List<EmployeeFavorite>> GetEmployeeFavoritesByMovieAsync(int movieId)
        {
            return await _context.Set<EmployeeFavorite>()
                .Include(ef => ef.AppUser)
                .Include(ef => ef.Movie)
                .Where(ef => ef.MovieId == movieId && ef.IsActive)
                .OrderByDescending(ef => ef.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Vérifie si un employé a déjà marqué un film comme coup de cœur.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est déjà marqué comme coup de cœur, sinon false.</returns>
        public async Task<bool> HasEmployeeFavoriteAsync(string appUserId, int movieId)
        {
            return await _context.Set<EmployeeFavorite>()
                .AnyAsync(ef => ef.AppUserId == appUserId && ef.MovieId == movieId && ef.IsActive);
        }

        /// <summary>
        /// Récupère tous les coups de cœur actifs.
        /// </summary>
        /// <returns>Une liste de coups de cœur actifs.</returns>
        public async Task<List<EmployeeFavorite>> GetAllActiveEmployeeFavoritesAsync()
        {
            return await _context.Set<EmployeeFavorite>()
                .Include(ef => ef.AppUser)
                .Include(ef => ef.Movie)
                .Where(ef => ef.IsActive)
                .OrderByDescending(ef => ef.CreatedAt)
                .ToListAsync();
        }
    }
}