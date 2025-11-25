using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public interface ITheaterRepository : IReadRepository<Theater>, IWriteRepository<Theater>
    {
        /// <summary>
        /// Récupère la liste des salles de cinéma associées à un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de salles de cinéma.</returns>
        Task<List<Theater>> GetTheatersByCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère la liste de tous les théâtres.
        /// </summary>
        /// <returns>Une liste de tous les théâtres.</returns>
        Task<List<Theater>> GetAllTheatersAsync();

        /// <summary>
        /// Récupère une salle de cinéma par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>La salle correspondante sous forme de DTO.</returns>
        Task<Theater> GetTheaterByIdAsync(int theaterId);

        /// <summary>
        /// Crée une nouvelle salle de cinéma.
        /// </summary>
        /// <param name="theater">L'objet Theater représentant la salle à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateTheaterAsync(Theater theater);

        /// <summary>
        /// Met à jour les informations d'une salle de cinéma existante.
        /// </summary>
        /// <param name="theater">L'objet Theater avec les nouvelles informations.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateTheaterAsync(Theater theater);

        /// <summary>
        /// Supprime une salle de cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteTheaterAsync(int theaterId);

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents.</returns>
        Task<List<Incident>> GetTheaterIncidentsAsync(int theaterId);
    }
    public class TheaterRepository : EFRepository<Theater>, ITheaterRepository
    {
        private readonly DbContext _context;

        public TheaterRepository(DbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        /// <summary>
        /// Récupère la liste des salles de cinéma associées à un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de salles de cinéma.</returns>
        public async Task<List<Theater>> GetTheatersByCinemaAsync(int cinemaId)
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Le contexte de la base de données n'est pas initialisé.");
            }

            // Récupérer les salles associées au cinéma spécifique
            var theaters = await _context.Set<Theater>()
                .Include(t => t.Cinema) // Charger Cinema
                .Where(t => t.CinemaId == cinemaId) // Filtrer par cinemaId
                .ToListAsync();

            return theaters;
        }

        /// <summary>
        /// Récupère la liste de tous les théâtres.
        /// </summary>
        /// <returns>Une liste de tous les théâtres.</returns>
        public async Task<List<Theater>> GetAllTheatersAsync()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Le contexte de la base de données n'est pas initialisé.");
            }

            // Récupérer toutes les salles avec leurs cinémas associés
            var theaters = await _context.Set<Theater>()
                .Include(t => t.Cinema) // Charger Cinema
                .Include(t => t.Seats)  // Charger les sièges si nécessaire
                .ToListAsync();

            return theaters;
        }

        /// <summary>
        /// Récupère une salle de cinéma par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>La salle de cinéma correspondante, ou null si elle n'existe pas.</returns>
        public async Task<Theater> GetTheaterByIdAsync(int theaterId)
        {
            return await _context.Set<Theater>()
                .Include(t => t.Cinema) // Inclure les détails du cinéma si nécessaire
                .Include(t => t.Seats)  // Inclure les sièges si nécessaire
                .FirstOrDefaultAsync(t => t.TheaterId == theaterId);
        }

        /// <summary>
        /// Crée une nouvelle salle de cinéma.
        /// </summary>
        /// <param name="theater">L'objet Theater représentant la salle à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CreateTheaterAsync(Theater theater)
        {
            if (theater == null)
            {
                throw new ArgumentNullException(nameof(theater));
            }

            theater.CreatedAt = DateTime.UtcNow;
            theater.UpdatedAt = DateTime.UtcNow;

            _context.Set<Theater>().Add(theater);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour les informations d'une salle de cinéma existante.
        /// </summary>
        /// <param name="theater">L'objet Theater avec les nouvelles informations.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateTheaterAsync(Theater theater)
        {
            _context.Set<Theater>().Update(theater);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime une salle de cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteTheaterAsync(int theaterId)
        {
            var theater = await _context.Set<Theater>().FindAsync(theaterId);
            if (theater == null)
            {
                throw new ArgumentException("Theater not found.");
            }

            _context.Set<Theater>().Remove(theater);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents.</returns>
        public async Task<List<Incident>> GetTheaterIncidentsAsync(int theaterId)
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Le contexte de la base de données n'est pas initialisé.");
            }

            // Inclure Cinema dans la requête
            var incidents = await _context.Set<Incident>()
                .Include(i => i.Theater)
                    .ThenInclude(t => t.Cinema)
                .Where(i => i.TheaterId == theaterId)
                .ToListAsync();

            return incidents;
        }
    }
}
