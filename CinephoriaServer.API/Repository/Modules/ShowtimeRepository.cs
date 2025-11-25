using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public interface IShowtimeRepository : IReadRepository<Showtime>, IWriteRepository<Showtime>
    {
        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtime">La séance à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateSessionAsync(Showtime showtime);

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtime">La séance à mettre à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateSessionAsync(Showtime showtime);

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteSessionAsync(int sessionId);

        Task<List<Showtime>> GetAllShowtimesAsync();
        Task<List<Showtime>> GetUpcomingShowtimesAsync();

        // Nouvelles méthodes pour la gestion des statuts
        Task<List<Showtime>> GetShowtimesByStatusAsync(EnumConfig.ShowtimeStatus status);
        Task<List<Showtime>> GetOngoingShowtimesAsync();
        Task<List<Showtime>> GetCompletedShowtimesAsync();
        Task<List<Showtime>> GetShowtimesWithMoviesAsync();
        Task<List<Showtime>> GetShowtimesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetShowtimeCountByStatusAsync(EnumConfig.ShowtimeStatus status);
        Task<List<Showtime>> GetShowtimesForStatusUpdateAsync();
        Task<List<object>> GetMoviesWithShowtimesAsync();
        Task<List<object>> GetRecentMoviesWithShowtimesAsync();
    }


    public class ShowtimeRepository : EFRepository<Showtime>, IShowtimeRepository
    {
        private readonly DbContext _context;

        public ShowtimeRepository(DbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtime">La séance à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CreateSessionAsync(Showtime showtime)
        {
            if (showtime == null)
            {
                throw new ArgumentNullException(nameof(showtime));
            }

            showtime.CreatedAt = DateTime.UtcNow;
            showtime.UpdatedAt = DateTime.UtcNow;

            _context.Set<Showtime>().Add(showtime);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtime">La séance à mettre à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateSessionAsync(Showtime showtime)
        {
            if (showtime == null)
            {
                throw new ArgumentNullException(nameof(showtime));
            }

            showtime.UpdatedAt = DateTime.UtcNow;

            _context.Set<Showtime>().Update(showtime);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteSessionAsync(int sessionId)
        {
            var showtime = await _context.Set<Showtime>().FindAsync(sessionId);
            if (showtime == null)
            {
                throw new ArgumentException("Séance non trouvée.");
            }

            _context.Set<Showtime>().Remove(showtime);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Showtime>> GetAllShowtimesAsync()
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetUpcomingShowtimesAsync()
        {
            var today = DateTime.Today;

            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.StartTime >= today)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetShowtimesByStatusAsync(EnumConfig.ShowtimeStatus status)
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.Status == status)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetOngoingShowtimesAsync()
        {
            var now = DateTime.Now;
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.StartTime <= now && s.EndTime >= now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetCompletedShowtimesAsync()
        {
            var now = DateTime.Now;
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.EndTime < now)
                .OrderByDescending(s => s.EndTime)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetShowtimesWithMoviesAsync()
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetShowtimesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<int> GetShowtimeCountByStatusAsync(EnumConfig.ShowtimeStatus status)
        {
            return await _context.Set<Showtime>()
                .Where(s => s.Status == status)
                .CountAsync();
        }

        public async Task<List<Showtime>> GetShowtimesForStatusUpdateAsync()
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.Status != EnumConfig.ShowtimeStatus.Cancelled)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<object>> GetMoviesWithShowtimesAsync()
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.Status != EnumConfig.ShowtimeStatus.Cancelled)
                .GroupBy(s => s.Movie)
                .Select(g => new
                {
                    MovieId = g.Key.MovieId,
                    MovieTitle = g.Key.Title,
                    ShowtimeCount = g.Count(),
                    NextShowtime = g.Min(s => s.StartTime)
                })
                .Cast<object>()
                .ToListAsync();
        }

        public async Task<List<object>> GetRecentMoviesWithShowtimesAsync()
        {
            // Calculer le dernier mercredi
            var today = DateTime.Today;
            var daysSinceWednesday = ((int)today.DayOfWeek - (int)DayOfWeek.Wednesday + 7) % 7;
            var lastWednesday = today.AddDays(-daysSinceWednesday);

            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.Movie.CreatedAt >= lastWednesday && s.Status != EnumConfig.ShowtimeStatus.Cancelled)
                .GroupBy(s => s.Movie)
                .Select(g => new
                {
                    MovieId = g.Key.MovieId,
                    MovieTitle = g.Key.Title,
                    CreatedAt = g.Key.CreatedAt,
                    ShowtimeCount = g.Count(),
                    NextShowtime = g.Min(s => s.StartTime)
                })
                .Cast<object>()
                .ToListAsync();
        }
    }
}
