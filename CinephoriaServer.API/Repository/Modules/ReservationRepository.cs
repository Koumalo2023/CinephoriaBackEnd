using CinephoriaServer.API.Data;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using CinephoriaServer.API.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Repository
{
    public interface IReservationRepository : IReadRepository<Reservation>, IWriteRepository<Reservation>
    {
        /// <summary>
        /// Récupère une réservation avec la liste des sièges.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la reservation.</param>
        /// <returns>Une liste de séances.</returns>
        Task<Reservation> GetByIdAsync(int reservationId);

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        Task<List<Showtime>> GetMovieSessionsAsync(int movieId);

        /// <summary>
        /// Récupère la liste de toutes les réservations d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de réservations.</returns>
        Task<List<Reservation>> GetReservationsByShowtimeAsync(int showtimeId);

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        Task<List<Seat>> GetAvailableSeatsAsync(int showtimeId);

        /// <summary>
        /// Calcule le prix total d'une réservation en fonction de la séance et des sièges sélectionnés.
        /// </summary>
        /// <param name="showtime">La séance pour laquelle la réservation est faite.</param>
        /// <param name="seats">La liste des sièges sélectionnés.</param>
        /// <returns>Le prix total de la réservation.</returns>
        Task<decimal> CalculateReservationPriceAsync(Showtime showtime, List<Seat> seats);

        /// <summary>
        /// Bloque des sièges pour une réservation en attente.
        /// </summary>
        /// <param name="showtime">La séance concernée.</param>
        /// <param name="seats">La liste des sièges à bloquer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task HoldSeatsAsync(int showtimeId, List<Seat> seats);

        /// <summary>
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        Task<List<Reservation>> GetUserReservationsAsync(string AppUserId);

        /// <summary>
        /// Crée une nouvelle réservation pour une séance spécifique.
        /// </summary>
        /// <param name="reservation">La réservation à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateReservationAsync(Reservation reservation);



        /// <summary>
        /// Libère les sièges réservés pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <param name="seatNumbers">La liste des numéros de sièges à libérer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task ReleaseSeatsAsync(int showtimeId, List<string> seatNumbers);

        /// <summary>
        /// Supprime une réservation existante.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteReservationAsync(int reservationId);
    }


    public class ReservationRepository : EFRepository<Reservation>, IReservationRepository
    {
        private readonly DbContext _context;
        private readonly QRCodeService _qrCodeService;

        public ReservationRepository(DbContext context, QRCodeService qrCodeService) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
           
        }
        /// <summary>
        /// Récupère une réservation avec la liste des sièges.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la reservation.</param>
        /// <returns>Une liste de séances.</returns>
        public async Task<Reservation> GetByIdAsync(int reservationId)
        {
            return await _context.Set<Reservation>()
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
        }

        /// <summary>
        /// Récupère la liste de toutes les réservations d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de réservations.</returns>
        public async Task<List<Reservation>> GetReservationsByShowtimeAsync(int showtimeId)
        {
            return await _context.Set<Reservation>()
                .Include(r => r.Seats) 
                .Where(r => r.ShowtimeId == showtimeId)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .Where(s => s.MovieId == movieId && s.StartTime >= DateTime.UtcNow)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        public async Task<List<Seat>> GetAvailableSeatsAsync(int showtimeId)
        {
            var showtime = await _context.Set<Showtime>()
                .Include(s => s.Theater)
                .ThenInclude(t => t.Seats)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                throw new ArgumentException("Séance non trouvée.");
            }

            return showtime.Theater.Seats
                .Where(s => s.IsAvailable)
                .ToList();
        }

        /// <summary>
        /// Calcule le prix total d'une réservation en fonction de la séance et des sièges sélectionnés.
        /// </summary>
        /// <param name="showtime">La séance pour laquelle la réservation est faite.</param>
        /// <param name="seats">La liste des sièges sélectionnés.</param>
        /// <returns>Le prix total de la réservation.</returns>
        public async Task<decimal> CalculateReservationPriceAsync(Showtime showtime, List<Seat> seats)
        {
            if (showtime == null || seats == null || !seats.Any())
            {
                throw new ArgumentException("La séance et les sièges doivent être valides.");
            }

            // Prix de base de la séance
            decimal basePrice = showtime.Price;

            // Appliquer les ajustements de prix (promotions, majorations, etc.)
            if (showtime.IsPromotion)
            {
                basePrice *= 0.9m; // 10% de réduction
            }

            // Calculer le prix total en fonction du nombre de sièges
            decimal totalPrice = basePrice * seats.Count;

            return totalPrice;
        }

        /// <summary>
        /// Crée une nouvelle réservation pour une séance spécifique.
        /// </summary>
        /// <param name="reservation">La réservation à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CreateReservationAsync(Reservation reservation)
        {
            if (reservation == null)
            {
                throw new ArgumentNullException(nameof(reservation));
            }

            // Vérifier que la séance est valide
            var showtime = await _context.Set<Showtime>()
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(s => s.ShowtimeId == reservation.ShowtimeId);

            if (showtime == null)
            {
                throw new ArgumentException("La séance spécifiée n'existe pas.");
            }

            // Charger les sièges à partir des numéros de sièges
            var seatNumbers = reservation.Seats.Select(s => s.SeatNumber).ToList();
            var seats = await _context.Set<Seat>()
                .Where(s => s.TheaterId == showtime.TheaterId && seatNumbers.Contains(s.SeatNumber))
                .ToListAsync();

            if (seats == null || !seats.Any())
            {
                throw new ArgumentException("Aucun siège trouvé avec les numéros fournis.");
            }


            // Associer les sièges chargés à la réservation
            reservation.Seats = seats;

            
            _context.Set<Reservation>().Add(reservation);
            await _context.SaveChangesAsync();

            // Générer le QRCode après l'enregistrement de la réservation
            byte[] qrCodeBytes = _qrCodeService.GenerateQRCode(reservation);

            // Convertir le QRCode en base64 pour le stocker dans la base de données
            reservation.QrCode = Convert.ToBase64String(qrCodeBytes);

            // Mettre à jour la réservation avec le QRCode généré
            _context.Set<Reservation>().Update(reservation);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Bloque des sièges pour une réservation en attente.
        /// </summary>
        /// <param name="showtime">La séance concernée.</param>
        /// <param name="seats">La liste des sièges à bloquer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task HoldSeatsAsync(int showtimeId, List<Seat> seats)
        {
            var showtime = await _context.Set<Showtime>()
                .Include(s => s.Theater)
                .ThenInclude(t => t.Seats)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                throw new ApiException("Séance non trouvée.", StatusCodes.Status404NotFound);
            }

            // Vérifier que les sièges existent et sont disponibles
            var unavailableSeats = seats.Where(s => !s.IsAvailable).ToList();
            if (unavailableSeats.Any())
            {
                throw new ApiException($"Les sièges suivants ne sont pas disponibles : {string.Join(", ", unavailableSeats.Select(s => s.SeatNumber))}", StatusCodes.Status400BadRequest);
            }

            // Bloquer les sièges
            foreach (var seat in seats)
            {
                seat.IsAvailable = false;
                _context.Set<Seat>().Update(seat);
            }

            showtime.AvailableSeats -= seats.Count;
            _context.Set<Showtime>().Update(showtime);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        public async Task<List<Reservation>> GetUserReservationsAsync(string AppUserId)
        {
            return await _context.Set<Reservation>()
                .Include(r => r.Showtime)
                    .ThenInclude(s => s.Movie) 
                .Include(r => r.Showtime)
                    .ThenInclude(s => s.Cinema)
                .Include(r => r.Seats)
                .Where(r => r.AppUserId == AppUserId)
                .ToListAsync();
        }

        /// <summary>
        /// Libère les sièges réservés pour une séance spécifique.
        /// </summary>
        public async Task ReleaseSeatsAsync(int showtimeId, List<string> seatNumbers)
        {
            var showtime = await _context.Set<Showtime>()
                .Include(s => s.Theater)
                .ThenInclude(t => t.Seats)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                throw new ApiException("Séance non trouvée.", StatusCodes.Status404NotFound);
            }

            var seats = showtime.Theater.Seats
                .Where(s => seatNumbers.Contains(s.SeatNumber))
                .ToList();

            if (seats == null || !seats.Any())
            {
                throw new ApiException("Aucun siège trouvé avec les numéros fournis.", StatusCodes.Status404NotFound);
            }

            foreach (var seat in seats)
            {
                seat.IsAvailable = true;
                _context.Set<Seat>().Update(seat);
            }

            showtime.AvailableSeats += seats.Count;
            _context.Set<Showtime>().Update(showtime);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime une réservation existante.
        /// </summary>
        public async Task DeleteReservationAsync(int reservationId)
        {
            var reservation = await _context.Set<Reservation>()
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                throw new ApiException("Réservation non trouvée.", StatusCodes.Status404NotFound);
            }

            // Libérer les sièges réservés
            await ReleaseSeatsAsync(reservation.ShowtimeId, reservation.Seats.Select(s => s.SeatNumber).ToList());

            // Rendre le QRCode null
            reservation.QrCode = null;

            // Supprimer la réservation
            _context.Set<Reservation>().Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
