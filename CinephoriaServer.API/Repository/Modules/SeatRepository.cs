using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public interface ISeatRepository : IReadRepository<Seat>, IWriteRepository<Seat>
    {
        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        Task<List<Seat>> GetAvailableSeatsAsync(int sessionId);

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à ajouter.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task AddHandicapSeatAsync(int theaterId, string seatNumber);

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task RemoveHandicapSeatAsync(int theaterId, string seatNumber);


        /// <summary>
        /// Récupère une liste de sièges en fonction de leurs numéros pour une séance donnée.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <param name="seatNumbers">La liste des numéros de sièges.</param>
        /// <returns>Une liste de sièges.</returns>
        Task<List<Seat>> GetSeatsByNumbersAsync(int showtimeId, List<string> seatNumbers);

        /// <summary>
        /// Ajoute une liste de sièges à la base de données.
        /// </summary>
        /// <param name="seats">La liste des sièges à ajouter.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task AddSeatsAsync(List<Seat> seats);

        /// <summary>
        /// Supprime tous les sièges associés à une salle spécifique.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteSeatsByTheaterIdAsync(int theaterId);

        /// <summary>
        /// Met à jour les informations d'un siège existant.
        /// </summary>
        /// <param name="seat">L'objet siège contenant les nouvelles informations.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateSeatAsync(Seat seat);

        /// <summary>
        /// Récupère tous les sièges d'une salle de cinéma par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>Une liste de sièges.</returns>
        Task<IEnumerable<Seat>> GetSeatsByTheaterIdAsync(int theaterId);

    }
    public class SeatRepository : EFRepository<Seat>, ISeatRepository
    {
        private readonly DbContext _context;

        public SeatRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        public async Task<List<Seat>> GetAvailableSeatsAsync(int sessionId)
        {
            // Récupérer les sièges réservés pour cette séance
            var reservedSeats = await _context.Set<Reservation>()
                .Where(r => r.ShowtimeId == sessionId)
                .SelectMany(r => r.Seats)
                .Select(s => s.SeatId)
                .ToListAsync();

            // Récupérer la salle associée à cette séance
            var showtime = await _context.Set<Showtime>()
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(s => s.ShowtimeId == sessionId);

            if (showtime == null)
            {
                throw new ArgumentException("Showtime not found.");
            }

            // Récupérer les sièges disponibles dans la salle
            return await _context.Set<Seat>()
                .Where(s => s.TheaterId == showtime.TheaterId && !reservedSeats.Contains(s.SeatId) && s.IsAvailable)
                .ToListAsync();
        }

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à ajouter.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task AddHandicapSeatAsync(int theaterId, string seatNumber)
        {
            var seat = new Seat
            {
                TheaterId = theaterId,
                SeatNumber = seatNumber,
                IsAccessible = true, // Marquer comme accessible
                IsAvailable = true,  // Disponible par défaut
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Set<Seat>().Add(seat);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task RemoveHandicapSeatAsync(int theaterId, string seatNumber)
        {
            var seat = await _context.Set<Seat>()
                .FirstOrDefaultAsync(s => s.TheaterId == theaterId && s.SeatNumber == seatNumber && s.IsAccessible);

            if (seat == null)
            {
                throw new ArgumentException("Handicap seat not found.");
            }

            _context.Set<Seat>().Remove(seat);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Récupère une liste de sièges en fonction de leurs numéros pour une séance donnée.
        /// </summary>
        public async Task<List<Seat>> GetSeatsByNumbersAsync(int showtimeId, List<string> seatNumbers)
        {
            // Récupérer la séance avec les sièges associés
            var showtime = await _context.Set<Showtime>()
                .Include(s => s.Theater)
                .ThenInclude(t => t.Seats)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                throw new ApiException("Séance non trouvée.", StatusCodes.Status404NotFound);
            }

            // Filtrer les sièges en fonction des numéros fournis
            var seats = showtime.Theater.Seats
                .Where(s => seatNumbers.Contains(s.SeatNumber))
                .ToList();

            if (seats == null || !seats.Any())
            {
                throw new ApiException("Aucun siège trouvé avec les numéros fournis.", StatusCodes.Status404NotFound);
            }

            return seats;
        }

        /// <summary>
        /// Ajoute une liste de sièges à la base de données.
        /// </summary>
        public async Task AddSeatsAsync(List<Seat> seats)
        {
            await _context.Set<Seat>().AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime tous les sièges associés à une salle spécifique.
        /// </summary>
        public async Task DeleteSeatsByTheaterIdAsync(int theaterId)
        {
            var seats = await _context.Set<Seat>()
                .Where(s => s.TheaterId == theaterId)
                .ToListAsync();

            _context.Set<Seat>().RemoveRange(seats);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour un siège existant dans la base de données.
        /// </summary>
        /// <param name="seat">L'objet siège contenant les nouvelles informations.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateSeatAsync(Seat seat)
        {
            var existingSeat = await _context.Set<Seat>().FindAsync(seat.SeatId);

            if (existingSeat == null)
            {
                throw new ArgumentException("Seat not found.");
            }

            // Mise à jour des propriétés
            existingSeat.SeatNumber = seat.SeatNumber;
            existingSeat.IsAccessible = seat.IsAccessible;
            existingSeat.IsAvailable = seat.IsAvailable;
            existingSeat.UpdatedAt = DateTime.UtcNow;

            _context.Set<Seat>().Update(existingSeat);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Récupère la liste des sièges d'une salle spécifique.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>Une liste de sièges sous forme de DTOs.</returns>
        public async Task<IEnumerable<Seat>> GetSeatsByTheaterIdAsync(int theaterId)
        {
            return await _context.Set<Seat>()
                .Where(seat => seat.TheaterId == theaterId)
                .ToListAsync();
        }

    }
}

