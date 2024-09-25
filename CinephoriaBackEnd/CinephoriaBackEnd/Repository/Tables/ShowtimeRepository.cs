using CinephoriaBackEnd.Data;
using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore;



namespace CinephoriaBackEnd.Repository
{
    /// <summary>
    /// Interface pour gérer les opérations liées aux séances
    /// </summary>
    public interface IShowtimeRepository
    {
        /// <summary>
        /// Récupérer la liste de toutes les séances disponibles
        /// </summary>
        /// <returns>Liste des séances</returns>
        Task<List<Showtime>> GetAllShowtimes(); // GET /api/seances

        /// <summary>
        /// Créer une nouvelle séance
        /// </summary>
        /// <param name="showtime">Les détails de la nouvelle séance</param>
        /// <returns>La séance créée</returns>
        Task<Showtime> CreateShowtime(Showtime showtime); // POST /api/seances

        /// <summary>
        /// Récupérer les détails d'une séance par son ID
        /// </summary>
        /// <param name="id">L'ID de la séance</param>
        /// <returns>Les détails de la séance</returns>
        Task<Showtime> GetShowtimeById(int id); // GET /api/seances/{id}

        /// <summary>
        /// Mettre à jour les informations d'une séance
        /// </summary>
        /// <param name="showtime">Les détails mis à jour de la séance</param>
        Task UpdateShowtime(Showtime showtime); // PUT /api/seances/{id}

        /// <summary>
        /// Supprimer une séance par son ID
        /// </summary>
        /// <param name="id">L'ID de la séance</param>
        Task DeleteShowtime(int id); // DELETE /api/seances/{id}

        /// <summary>
        /// Récupérer les séances réservées par un utilisateur spécifique
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur</param>
        /// <returns>Liste des séances de l'utilisateur</returns>
        Task<List<Showtime>> GetUserShowtimes(string userId); // GET /api/seances/user/{userId}
    }

    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly CinephoriaDbContext _context;

        public ShowtimeRepository(CinephoriaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupérer la liste de toutes les séances disponibles
        /// </summary>
        /// <returns>Liste des séances</returns>
        public async Task<List<Showtime>> GetAllShowtimes()
        {
            return await _context.Showtimes.ToListAsync();
        }

        /// <summary>
        /// Créer une nouvelle séance
        /// </summary>
        /// <param name="showtime">Les détails de la nouvelle séance</param>
        /// <returns>La séance créée</returns>
        public async Task<Showtime> CreateShowtime(Showtime showtime)
        {
            await _context.Showtimes.AddAsync(showtime);
            await _context.SaveChangesAsync();
            return showtime;
        }

        /// <summary>
        /// Récupérer les détails d'une séance par son ID
        /// </summary>
        /// <param name="id">L'ID de la séance</param>
        /// <returns>Les détails de la séance</returns>
        public async Task<Showtime> GetShowtimeById(int id)
        {
            return await _context.Showtimes.FindAsync(id);
        }

        /// <summary>
        /// Mettre à jour les informations d'une séance
        /// </summary>
        /// <param name="showtime">Les détails mis à jour de la séance</param>
        public async Task UpdateShowtime(Showtime showtime)
        {
            _context.Showtimes.Update(showtime);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprimer une séance par son ID
        /// </summary>
        /// <param name="id">L'ID de la séance</param>
        public async Task DeleteShowtime(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime != null)
            {
                _context.Showtimes.Remove(showtime);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Récupérer les séances réservées par un utilisateur spécifique
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur</param>
        /// <returns>Liste des séances de l'utilisateur</returns>
        public async Task<List<Showtime>> GetUserShowtimes(string userId)
        {
            return await _context.Showtimes.Where(s => s.AppUserId == userId).ToListAsync();
        }
    }

}
