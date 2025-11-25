using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CinephoriaServer.API.Repository
{
    public interface ICinemaRepository : IReadRepository<Cinema>, IWriteRepository<Cinema>
    {
        /// <summary>
        /// Récupère la liste de tous les cinémas disponibles.
        /// </summary>
        /// <returns>Une liste de cinémas.</returns>
        Task<List<Cinema>> GetAllCinemasAsync();

        /// <summary>
        /// Récupère un cinéma par son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Le cinéma correspondant, ou null s'il n'existe pas.</returns>
        Task<Cinema> GetCinemaByIdAsync(int cinemaId);

        /// <summary>
        /// Récupère la liste des films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films.</returns>
        Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId);

        /// <summary>
        /// Crée un nouveau cinéma.
        /// </summary>
        /// <param name="cinema">Le cinéma à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateCinemaAsync(Cinema cinema);

        /// <summary>
        /// Met à jour les informations d'un cinéma existant.
        /// </summary>
        /// <param name="cinema">Le cinéma à mettre à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateCinemaAsync(Cinema cinema);

        /// <summary>
        /// Supprime un cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteCinemaAsync(int cinemaId);
    }

    public class CinemaRepository : EFRepository<Cinema>, ICinemaRepository
    {
        public CinemaRepository(DbContext context) : base(context) { }

        /// <summary>
        /// Récupère tous les cinémas disponibles.
        /// </summary>
        /// <returns>Une liste de cinémas.</returns>
        public async Task<List<Cinema>> GetAllCinemasAsync()
        {
            return await _context.Set<Cinema>()
                .Include(c => c.Theaters) // Inclure les salles associées à chaque cinéma
                .ToListAsync();
        }

        /// <summary>
        /// Récupère un cinéma par son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Le cinéma correspondant, ou null s'il n'existe pas.</returns>
        public async Task<Cinema> GetCinemaByIdAsync(int cinemaId)
        {
            return await _context.Set<Cinema>()
                .Include(c => c.Theaters) // Inclure les salles associées
                .FirstOrDefaultAsync(c => c.CinemaId == cinemaId);
        }

        /// <summary>
        /// Récupère la liste des films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films.</returns>
        public async Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId)
        {
            return await _context.Set<Showtime>()
                .Where(s => s.Theater.CinemaId == cinemaId) // Filtrer par cinéma
                .Select(s => s.Movie) // Sélectionner les films associés aux séances
                .Distinct() // Éviter les doublons
                .ToListAsync();
        }

        /// <summary>
        /// Crée un nouveau cinéma.
        /// </summary>
        /// <param name="cinema">Le cinéma à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CreateCinemaAsync(Cinema cinema)
        {
            cinema.CreatedAt = DateTime.UtcNow;
            cinema.UpdatedAt = DateTime.UtcNow;

            _context.Set<Cinema>().Add(cinema);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour les informations d'un cinéma existant.
        /// </summary>
        /// <param name="cinema">Le cinéma à mettre à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateCinemaAsync(Cinema cinema)
        {
            cinema.UpdatedAt = DateTime.UtcNow;

            _context.Set<Cinema>().Update(cinema);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime un cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteCinemaAsync(int cinemaId)
        {
            var cinema = await _context.Set<Cinema>().FindAsync(cinemaId);
            if (cinema == null)
            {
                throw new ArgumentException("Cinema not found.");
            }

            _context.Set<Cinema>().Remove(cinema);
            await _context.SaveChangesAsync();
        }
    }
}
