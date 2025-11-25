using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IReservationReminderService _reminderService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(
            IReservationService reservationService,
            IReservationReminderService reminderService,
            IEmailService emailService,
            ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;
            _reminderService = reminderService;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances disponibles.</returns>
        [HttpGet("movie/{movieId}/sessions")]
        public async Task<IActionResult> GetMovieSessions(int movieId)
        {
            try
            {
                var sessions = await _reservationService.GetMovieSessionsAsync(movieId);
                return Ok(sessions);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        [HttpGet("showtime/{showtimeId}/seats")]
        public async Task<IActionResult> GetAvailableSeats(int showtimeId)
        {
            try
            {
                var seats = await _reservationService.GetAvailableSeatsAsync(showtimeId);
                return Ok(seats);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations sous forme de DTO.</returns>
        [HttpGet("user/{AppUserId}")]
        public async Task<IActionResult> GetUserReservations(string AppUserId)
        {
            try
            {
                var reservations = await _reservationService.GetUserReservationsAsync(AppUserId);
                return Ok(reservations);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste de toutes les réservations d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de réservations.</returns>
        [HttpGet("showtime/{showtimeId}")]
        public async Task<ActionResult<List<ReservationDto>>> GetReservationsByShowtime(int showtimeId)
        {
            try
            {
                // Appeler le service pour récupérer les réservations
                var reservations = await _reservationService.GetReservationsByShowtimeAsync(showtimeId);

                if (reservations == null || !reservations.Any())
                {
                    return NotFound("Aucune réservation trouvée pour cette séance.");
                }

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        /// <summary>
        /// Valide un QRCode scanné pour une réservation.
        /// </summary>
        /// <param name="qrCodeData">Les données du QRCode scanné.</param>
        /// <returns>Une réponse indiquant si la validation a réussi.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateSession([FromBody] string qrCodeData)
        {
            try
            {
                bool isValid = await _reservationService.ValidatedSession(qrCodeData);

                if (isValid)
                {
                    return Ok("QRCode validé avec succès.");
                }
                else
                {
                    return BadRequest("Validation du QRCode échouée.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Crée une nouvelle réservation.
        /// </summary>
        /// <param name="createReservationDto">Les données de la réservation à créer.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto createReservationDto)
        {
            try
            {
                var result = await _reservationService.CreateReservationAsync(createReservationDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Annule une réservation existante.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à annuler.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpDelete("cancel/{reservationId}")]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            try
            {
                var result = await _reservationService.CancelReservationAsync(reservationId);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère une réservation spécifique par son identifiant
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation</param>
        /// <returns>Les détails de la réservation</returns>
        [HttpGet("{reservationId}")]
        public async Task<ActionResult<ReservationDto>> GetReservationById(int reservationId)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(reservationId);

                if (reservation == null)
                {
                    return NotFound($"Aucune réservation trouvée avec l'identifiant {reservationId}.");
                }

                return Ok(reservation);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la réservation {ReservationId}", reservationId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la récupération de la réservation." });
            }
        }

        #region Reservation Reminder Methods

        /// <summary>
        /// Envoie manuellement les rappels de paiement
        /// </summary>
        [HttpPost("send-payment-reminders")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ReminderResultDto>> SendPaymentReminders()
        {
            try
            {
                var count = await _reminderService.SendPaymentRemindersAsync();
                _logger.LogInformation("Rappels de paiement envoyés manuellement : {Count}", count);
                
                return Ok(new ReminderResultDto
                {
                    Message = "Rappels de paiement envoyés avec succès",
                    Count = count,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi manuel des rappels de paiement");
                return StatusCode(500, new ReminderResultDto
                {
                    Message = "Erreur lors de l'envoi des rappels de paiement",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Envoie manuellement les rappels de séance
        /// </summary>
        [HttpPost("send-showtime-reminders")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ReminderResultDto>> SendShowtimeReminders()
        {
            try
            {
                var count = await _reminderService.SendUpcomingShowtimeRemindersAsync();
                _logger.LogInformation("Rappels de séance envoyés manuellement : {Count}", count);
                
                return Ok(new ReminderResultDto
                {
                    Message = "Rappels de séance envoyés avec succès",
                    Count = count,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi manuel des rappels de séance");
                return StatusCode(500, new ReminderResultDto
                {
                    Message = "Erreur lors de l'envoi des rappels de séance",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Envoie manuellement les avertissements d'expiration
        /// </summary>
        [HttpPost("send-expiration-warnings")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ReminderResultDto>> SendExpirationWarnings()
        {
            try
            {
                var count = await _reminderService.SendExpirationWarningsAsync();
                _logger.LogInformation("Avertissements d'expiration envoyés manuellement : {Count}", count);
                
                return Ok(new ReminderResultDto
                {
                    Message = "Avertissements d'expiration envoyés avec succès",
                    Count = count,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi manuel des avertissements d'expiration");
                return StatusCode(500, new ReminderResultDto
                {
                    Message = "Erreur lors de l'envoi des avertissements d'expiration",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Récupère les statistiques des rappels
        /// </summary>
        [HttpGet("reminder-stats")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ReminderStatsDto>> GetReminderStats()
        {
            try
            {
                var stats = await _reminderService.GetReminderStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques des rappels");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère les statistiques d'email
        /// </summary>
        [HttpGet("email-stats")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<EmailStatsDto>> GetEmailStats()
        {
            try
            {
                var stats = await _emailService.GetEmailStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques d'email");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Teste la configuration du service d'email
        /// </summary>
        [HttpPost("test-email-config")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<TestEmailResultDto>> TestEmailConfiguration()
        {
            try
            {
                var isConfigured = await _emailService.ValidateConfigurationAsync();
                
                if (isConfigured)
                {
                    // Test d'envoi d'email
                    var testEmail = "test@cinephoria.com";
                    var testSubject = "Test de configuration - Cinephoria";
                    var testBody = @"
                        <h2>Test de configuration du service d'email</h2>
                        <p>Ceci est un email de test envoyé par le système Cinephoria.</p>
                        <p>Si vous recevez cet email, la configuration est correcte.</p>
                        <p>Date d'envoi : " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + @"</p>
                    ";

                    var success = await _emailService.SendEmailAsync(testEmail, testSubject, testBody, true);

                    return Ok(new TestEmailResultDto
                    {
                        IsConfigured = true,
                        TestEmailSent = success,
                        Message = success ?
                            "Configuration valide et email de test envoyé avec succès" :
                            "Configuration valide mais échec de l'envoi de l'email de test",
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return BadRequest(new TestEmailResultDto
                    {
                        IsConfigured = false,
                        TestEmailSent = false,
                        Message = "Configuration du service d'email invalide",
                        Timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du test de configuration d'email");
                return StatusCode(500, new TestEmailResultDto
                {
                    IsConfigured = false,
                    TestEmailSent = false,
                    Message = "Erreur lors du test de configuration",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Récupère les réservations nécessitant des rappels de paiement
        /// </summary>
        [HttpGet("pending-payment-reminders")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<ReservationReminderDto>>> GetPendingPaymentReminders()
        {
            try
            {
                var reservations = await _reminderService.GetReservationsNeedingPaymentRemindersAsync();
                var result = reservations.Select(r => new ReservationReminderDto
                {
                    ReservationId = r.ReservationId,
                    UserEmail = r.AppUser?.Email ?? "Email non disponible",
                    MovieTitle = r.Showtime?.Movie?.Title ?? "Film inconnu",
                    ShowtimeDate = r.Showtime?.StartTime ?? DateTime.MinValue,
                    CreatedAt = r.CreatedAt,
                    LastReminderSent = r.LastReminderSent,
                    Status = r.Status.ToString()
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rappels de paiement en attente");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère les réservations nécessitant des rappels de séance
        /// </summary>
        [HttpGet("pending-showtime-reminders")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<ReservationReminderDto>>> GetPendingShowtimeReminders()
        {
            try
            {
                var reservations = await _reminderService.GetReservationsNeedingShowtimeRemindersAsync();
                var result = reservations.Select(r => new ReservationReminderDto
                {
                    ReservationId = r.ReservationId,
                    UserEmail = r.AppUser?.Email ?? "Email non disponible",
                    MovieTitle = r.Showtime?.Movie?.Title ?? "Film inconnu",
                    ShowtimeDate = r.Showtime?.StartTime ?? DateTime.MinValue,
                    CreatedAt = r.CreatedAt,
                    LastReminderSent = r.LastReminderSent,
                    Status = r.Status.ToString()
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rappels de séance en attente");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion

        #region DTO Classes

        public class ReminderResultDto
        {
            public string Message { get; set; } = string.Empty;
            public int Count { get; set; }
            public string? Error { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public class TestEmailResultDto
        {
            public bool IsConfigured { get; set; }
            public bool TestEmailSent { get; set; }
            public string Message { get; set; } = string.Empty;
            public string? Error { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public class ReservationReminderDto
        {
            public int ReservationId { get; set; }
            public string UserEmail { get; set; } = string.Empty;
            public string MovieTitle { get; set; } = string.Empty;
            public DateTime ShowtimeDate { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? LastReminderSent { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        #endregion
    }
}
