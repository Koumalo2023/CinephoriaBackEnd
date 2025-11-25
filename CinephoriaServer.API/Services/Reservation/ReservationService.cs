using AutoMapper;
using CinephoriaServer.API.Data;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using CinephoriaServer.API.Services.Notification;
using Microsoft.EntityFrameworkCore;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly CinephoriaDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationService> _logger;
        private readonly INotificationService _notificationService;

        public ReservationService(
            IUnitOfWorkPostgres unitOfWork,
            IMapper mapper,
            ILogger<ReservationService> logger,
            CinephoriaDbContext context,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        public async Task<List<ShowtimeDto>> GetMovieSessionsAsync(int movieId)
        {
            var showtimes = await _unitOfWork.Reservations.GetMovieSessionsAsync(movieId);
            return _mapper.Map<List<ShowtimeDto>>(showtimes);
        }


        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        public async Task<List<SeatDto>> GetAvailableSeatsAsync(int showtimeId)
        {
            var seats = await _unitOfWork.Seats.GetAvailableSeatsAsync(showtimeId);
            return _mapper.Map<List<SeatDto>>(seats);
        }


        /// <summary>
        /// Crée une nouvelle réservation.
        /// </summary>
        public async Task<string> CreateReservationAsync(CreateReservationDto createReservationDto)
        {
            if (createReservationDto == null)
            {
                throw new ApiException("Les données de la réservation sont invalides.", StatusCodes.Status400BadRequest);
            }

            // Récupérer la séance
            var showtime = await _unitOfWork.Showtimes.GetByIdAsync(createReservationDto.ShowtimeId);
            if (showtime == null)
            {
                throw new ApiException("Séance non trouvée.", StatusCodes.Status404NotFound);
            }

            // Récupérer les sièges correspondant aux numéros fournis
            var seats = await _unitOfWork.Seats.GetSeatsByNumbersAsync(createReservationDto.ShowtimeId, createReservationDto.SeatNumbers.ToList());
            if (seats == null || !seats.Any())
            {
                throw new ApiException("Aucun siège trouvé avec les numéros fournis.", StatusCodes.Status404NotFound);
            }

            // Bloquer les sièges
            await _unitOfWork.Reservations.HoldSeatsAsync(showtime.ShowtimeId, seats);

            // Créer la réservation
            var reservation = _mapper.Map<CinephoriaServer.API.Models.PostgresqlDb.Reservation>(createReservationDto);
            reservation.TotalPrice = (float)await _unitOfWork.Reservations.CalculateReservationPriceAsync(showtime, seats);
            reservation.Status = ReservationStatus.Confirmed;

            // Associer les sièges à la réservation
            reservation.Seats = seats;

            // Enregistrer la réservation
            await _unitOfWork.Reservations.CreateReservationAsync(reservation);

            // Mapper la réservation vers DTO pour la notification
            var reservationDto = _mapper.Map<ReservationDto>(reservation);

            // Envoyer la notification de nouvelle réservation
            await _notificationService.SendNewReservationNotificationAsync(createReservationDto.AppUserId, reservationDto);

            _logger.LogInformation("Réservation créée et confirmée avec succès pour l'utilisateur avec l'ID {AppUserId}.", createReservationDto.AppUserId);
            return "Réservation créée et confirmée avec succès.";
        }

        /// <summary>
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        public async Task<List<UserReservationDto>> GetUserReservationsAsync(string AppUserId)
        {
            if (string.IsNullOrEmpty(AppUserId))
            {
                throw new ApiException("L'identifiant de l'utilisateur est invalide.", StatusCodes.Status400BadRequest);
            }

            // Récupérer les réservations de l'utilisateur
            var reservations = await _unitOfWork.Reservations.GetUserReservationsAsync(AppUserId);

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation("Aucune réservation trouvée pour l'utilisateur avec l'ID {AppUserId}.", AppUserId);
                return new List<UserReservationDto>(); // Retourner une liste vide au lieu de null
            }

            // Mapper les réservations vers des DTO
            var userReservationDtos = _mapper.Map<List<UserReservationDto>>(reservations);

            if (userReservationDtos == null)
            {
                _logger.LogError("Erreur lors du mapping des réservations.");
                throw new ApiException("Erreur lors du traitement des réservations.", StatusCodes.Status500InternalServerError);
            }

            _logger.LogInformation("{Count} réservations récupérées pour l'utilisateur avec l'ID {AppUserId}.", userReservationDtos.Count, AppUserId);
            return userReservationDtos;
        }

        /// <summary>
        /// Récupère une réservation spécifique par son identifiant
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation</param>
        /// <returns>Les détails de la réservation</returns>
        public async Task<ReservationDto?> GetReservationByIdAsync(int reservationId)
        {
            if (reservationId <= 0)
            {
                throw new ApiException("L'identifiant de la réservation doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            // Récupérer la réservation avec les détails associés
            var reservation = await _context.Set<CinephoriaServer.API.Models.PostgresqlDb.Reservation>()
                .Include(r => r.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Seats)
                .Include(r => r.AppUser)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                _logger.LogInformation("Aucune réservation trouvée avec l'ID {ReservationId}.", reservationId);
                return null;
            }

            // Mapper la réservation vers DTO
            var reservationDto = _mapper.Map<ReservationDto>(reservation);

            _logger.LogInformation("Réservation avec l'ID {ReservationId} récupérée avec succès.", reservationId);
            return reservationDto;
        }


        /// <summary>
        /// Récupère la liste de toutes les réservations d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de réservations.</returns>
        public async Task<List<ReservationDto>> GetReservationsByShowtimeAsync(int showtimeId)
        {
            // Récupérer les réservations associées à la séance
            var reservations = await _unitOfWork.Reservations.GetReservationsByShowtimeAsync(showtimeId);

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation("Aucune réservation trouvée pour la séance avec l'ID {ShowtimeId}.", showtimeId);
                return new List<ReservationDto>();
            }

            // Mapper les réservations vers des DTO
            var reservationDtos = _mapper.Map<List<ReservationDto>>(reservations);

            return reservationDtos;
        }


        /// <summary>
        /// Annule une réservation existante.
        /// </summary>
        public async Task<string> CancelReservationAsync(int reservationId)
        {
            if (reservationId <= 0)
            {
                throw new ApiException("L'identifiant de la réservation doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            // Récupérer la réservation avec les sièges associés
            var reservation = await _context.Set<CinephoriaServer.API.Models.PostgresqlDb.Reservation>()
                .Include(r => r.Seats) // Inclure les sièges associés
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                throw new ApiException("Réservation non trouvée.", StatusCodes.Status404NotFound);
            }

            // Extraire les numéros de sièges
            var seatNumbers = reservation.Seats.Select(s => s.SeatNumber).ToList();

            // Libérer les sièges
            await _unitOfWork.Reservations.ReleaseSeatsAsync(reservation.ShowtimeId, seatNumbers);

            // Mapper la réservation vers DTO pour la notification
            var reservationDto = _mapper.Map<ReservationDto>(reservation);

            // Supprimer la réservation
            await _unitOfWork.Reservations.DeleteReservationAsync(reservationId);

            // Envoyer la notification d'annulation de réservation
            await _notificationService.SendCanceledReservationNotificationAsync(reservation.AppUserId, reservationDto);

            _logger.LogInformation("Réservation avec l'ID {ReservationId} annulée avec succès.", reservationId);
            return "Réservation annulée avec succès.";
        }


        /// <summary>
        /// Calcule le prix total d'une réservation en fonction de la séance et des sièges sélectionnés.
        /// </summary>
        public async Task<decimal> CalculateReservationPriceAsync(int showtimeId, List<string> seatNumbers)
        {
            var showtime = await _unitOfWork.Showtimes.GetByIdAsync(showtimeId);
            var seats = await _unitOfWork.Seats.GetSeatsByNumbersAsync(showtimeId, seatNumbers);

            if (showtime == null || seats == null || !seats.Any())
            {
                throw new ApiException("La séance ou les sièges sélectionnés sont invalides.", StatusCodes.Status400BadRequest);
            }

            return await _unitOfWork.Reservations.CalculateReservationPriceAsync(showtime, seats);
        }


        /// <summary>
        /// Bloque des sièges pour une réservation en attente.
        /// </summary>
        public async Task HoldSeatsAsync(int showtimeId, List<string> seatNumbers)
        {
            // Récupérer la séance
            var showtime = await _unitOfWork.Showtimes.GetByIdAsync(showtimeId);
            if (showtime == null)
            {
                throw new ApiException("Séance non trouvée.", StatusCodes.Status404NotFound);
            }

            // Récupérer les sièges correspondant aux numéros fournis
            var seats = await _unitOfWork.Seats.GetSeatsByNumbersAsync(showtimeId, seatNumbers);
            if (seats == null || !seats.Any())
            {
                throw new ApiException("Aucun siège trouvé avec les numéros fournis.", StatusCodes.Status404NotFound);
            }

            // Bloquer les sièges
            await _unitOfWork.Reservations.HoldSeatsAsync(showtimeId, seats);

            _logger.LogInformation("Sièges bloqués avec succès pour la séance avec l'ID {ShowtimeId}.", showtimeId);
        }

        /// <summary>
        /// Valide un QRCode scanné pour une réservation.
        /// </summary>
        /// <param name="qrCodeData">Les données du QRCode scanné.</param>
        /// <returns>True si la validation est réussie, sinon False.</returns>
        public async Task<bool> ValidatedSession(string qrCodeData)
        {
            if (string.IsNullOrEmpty(qrCodeData))
            {
                throw new ArgumentException("Les données du QRCode sont invalides.");
            }

            // Décoder les données du QRCode
            var qrCodeParts = qrCodeData.Split(';');
            if (qrCodeParts.Length != 3)
            {
                throw new ArgumentException("Format du QRCode invalide.");
            }

            // Extraire les informations du QRCode
            var reservationIdPart = qrCodeParts[0].Split(':');
            var showtimeIdPart = qrCodeParts[1].Split(':');
            var AppUserIdPart = qrCodeParts[2].Split(':');

            if (reservationIdPart.Length != 2 || showtimeIdPart.Length != 2 || AppUserIdPart.Length != 2)
            {
                throw new ArgumentException("Format du QRCode invalide.");
            }

            int reservationId = int.Parse(reservationIdPart[1]);
            int showtimeId = int.Parse(showtimeIdPart[1]);
            string AppUserId = AppUserIdPart[1];

            // Récupérer la réservation correspondante avec une requête personnalisée
            var reservation = await _context.Reservations
                .Include(res => res.Showtime)
                .FirstOrDefaultAsync(res => res.ReservationId == reservationId);

            if (reservation == null)
            {
                throw new ArgumentException("Réservation non trouvée.");
            }

            // Vérifier si la séance est encore valide (par exemple, la séance n'a pas encore commencé)
            if (reservation.Showtime.StartTime < DateTime.UtcNow)
            {
                throw new InvalidOperationException("La séance a déjà commencé.");
            }

            // Marquer la réservation comme validée
            reservation.IsValidated = true;

            // Mettre à jour la réservation dans la base de données
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Réservation avec l'ID {ReservationId} validée avec succès.", reservationId);
            return true;
        }

    }

}
