using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Repository
{
    public interface IUserRepository : IReadRepository<AppUser>, IWriteRepository<AppUser>
    {
        /// <summary>
        /// Récupère le profil d'un utilisateur en fonction de son identifiant.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur.</returns>
        Task<AppUser> GetUserProfileAsync(string AppUserId);

        /// <summary>
        /// Récupère la liste des commandes (réservations) d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        Task<List<Reservation>> GetUserOrdersAsync(string AppUserId);

        /// <summary>
        /// Récupère le profil d'un employé avec la liste des incidents qu'il a gérés.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé avec la liste des incidents.</returns>
        Task<AppUser> GetEmployeeProfileAsync(string appUserId);

        /// <summary>
        /// Ajoute un film aux favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        Task AddMovieToFavoritesAsync(string userId, int movieId);

        /// <summary>
        /// Supprime un film des favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        Task RemoveMovieFromFavoritesAsync(string userId, int movieId);

        /// <summary>
        /// Récupère la liste des films favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>La liste des films favoris.</returns>
        Task<List<Movie>> GetUserFavoriteMoviesAsync(string userId);

        /// <summary>
        /// Vérifie si un film est dans les favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est dans les favoris, sinon false.</returns>
        Task<bool> IsMovieInFavoritesAsync(string userId, int movieId);

        /// <summary>
        /// Récupère un utilisateur par son identifiant (string).
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur.</param>
        /// <returns>L'utilisateur correspondant.</returns>
        Task<AppUser> GetByIdAsync(string id);

    }

    public class UserRepository : EFRepository<AppUser>, IUserRepository
    {
        public UserRepository(CinephoriaDbContext context) : base(context) { }

        /// <summary>
        /// Récupère le profil d'un utilisateur en fonction de son identifiant.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur.</returns>
        public async Task<AppUser> GetUserProfileAsync(string AppUserId)
        {
            return await _context.Set<AppUser>()
                .Include(u => u.Reservations)
                .Include(u => u.MovieRatings)
                .FirstOrDefaultAsync(u => u.Id == AppUserId);
        }

        /// <summary>
        /// Récupère la liste des commandes (réservations) d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        public async Task<List<Reservation>> GetUserOrdersAsync(string AppUserId)
        {
            return await _context.Set<Reservation>()
                .Where(r => r.AppUserId == AppUserId)
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Movie)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère le profil d'un employé avec la liste des incidents qu'il a gérés.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé avec la liste des incidents.</returns>
        public async Task<AppUser> GetEmployeeProfileAsync(string appUserId)
        {
            // Récupérer l'utilisateur avec les incidents signalés et les informations du théâtre
            var employee = await _context.Set<AppUser>()
                .Include(u => u.ReportedIncidents)
                .Include(u => u.ResolvedByIncidents)
                    .ThenInclude(i => i.Theater)
                .FirstOrDefaultAsync(u => u.Id == appUserId && (u.Role == UserRole.Employee || u.Role == UserRole.Admin));

            if (employee == null)
            {
                throw new NotFoundException("Utilisateur non trouvé ou n'est pas un employé ou un administrateur.");
            }

            return employee;
        }

        /// <summary>
        /// Ajoute un film aux favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        public async Task AddMovieToFavoritesAsync(string userId, int movieId)
        {
            var existingFavorite = await _context.Set<UserFavorite>()
                .FirstOrDefaultAsync(uf => uf.AppUserId == userId && uf.MovieId == movieId);

            if (existingFavorite != null)
                return; // Déjà dans les favoris

            var userFavorite = new UserFavorite
            {
                AppUserId = userId,
                MovieId = movieId,
                AddedAt = DateTime.UtcNow
            };

            await _context.Set<UserFavorite>().AddAsync(userFavorite);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime un film des favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        public async Task RemoveMovieFromFavoritesAsync(string userId, int movieId)
        {
            var userFavorite = await _context.Set<UserFavorite>()
                .FirstOrDefaultAsync(uf => uf.AppUserId == userId && uf.MovieId == movieId);

            if (userFavorite != null)
            {
                _context.Set<UserFavorite>().Remove(userFavorite);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Récupère la liste des films favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>La liste des films favoris.</returns>
        public async Task<List<Movie>> GetUserFavoriteMoviesAsync(string userId)
        {
            return await _context.Set<UserFavorite>()
                .Where(uf => uf.AppUserId == userId)
                .Include(uf => uf.Movie)
                .Select(uf => uf.Movie)
                .ToListAsync();
        }

        /// <summary>
        /// Vérifie si un film est dans les favoris d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>True si le film est dans les favoris, sinon false.</returns>
        public async Task<bool> IsMovieInFavoritesAsync(string userId, int movieId)
        {
            return await _context.Set<UserFavorite>()
                .AnyAsync(uf => uf.AppUserId == userId && uf.MovieId == movieId);
        }

        /// <summary>
        /// Récupère un utilisateur par son identifiant (string).
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur.</param>
        /// <returns>L'utilisateur correspondant.</returns>
        public async Task<AppUser> GetByIdAsync(string id)
        {
            return await _context.Set<AppUser>().FindAsync(id);
        }
    }
}
