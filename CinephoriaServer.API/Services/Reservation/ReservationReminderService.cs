using CinephoriaServer.API.Configurations; 
using CinephoriaServer.API.Repository; 
using System.Text;

namespace CinephoriaServer.API.Services
{
    public class ReservationReminderService : IReservationReminderService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly ILogger<ReservationReminderService> _logger;
        private readonly IEmailService _emailService;
        private readonly ReminderStatsDto _stats;

        public ReservationReminderService(
            IUnitOfWorkPostgres unitOfWork,
            ILogger<ReservationReminderService> logger,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailService = emailService;
            _stats = new ReminderStatsDto { LastUpdate = DateTime.UtcNow };
        }

        public async Task<int> SendPaymentRemindersAsync()
        {
            try
            {
                var reservations = await GetReservationsNeedingPaymentRemindersAsync();
                var sentCount = 0;

                foreach (var reservation in reservations)
                {
                    try
                    {
                        var template = EmailTemplates.PaymentReminderTemplate;
                        var emailBody = FormatEmailBody(template.Body, reservation);
                        
                        var success = await _emailService.SendEmailAsync(
                            reservation.AppUser?.Email ?? string.Empty,
                            template.Subject,
                            emailBody,
                            template.IsHtml
                        );

                        if (success)
                        {
                            reservation.LastReminderSent = DateTime.UtcNow;
                            await _unitOfWork.Reservations.UpdateAsync(reservation);
                            sentCount++;
                            _stats.PaymentRemindersSent++;
                            _stats.TotalSentToday++;
                            
                            _logger.LogInformation("Rappel de paiement envoyé pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                        else
                        {
                            _stats.FailedSends++;
                            _logger.LogWarning("Échec d'envoi du rappel de paiement pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _stats.FailedSends++;
                        _logger.LogError(ex, "Erreur lors de l'envoi du rappel de paiement pour la réservation {ReservationId}", reservation.ReservationId);
                    }
                }

                _stats.LastUpdate = DateTime.UtcNow;
                return sentCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des rappels de paiement");
                throw;
            }
        }

        public async Task<int> SendUpcomingShowtimeRemindersAsync()
        {
            try
            {
                var reservations = await GetReservationsNeedingShowtimeRemindersAsync();
                var sentCount = 0;

                foreach (var reservation in reservations)
                {
                    try
                    {
                        var template = EmailTemplates.ShowtimeReminderTemplate;
                        var emailBody = FormatEmailBody(template.Body, reservation);
                        
                        var success = await _emailService.SendEmailAsync(
                            reservation.AppUser?.Email ?? string.Empty,
                            template.Subject,
                            emailBody,
                            template.IsHtml
                        );

                        if (success)
                        {
                            reservation.LastReminderSent = DateTime.UtcNow;
                            await _unitOfWork.Reservations.UpdateAsync(reservation);
                            sentCount++;
                            _stats.ShowtimeRemindersSent++;
                            _stats.TotalSentToday++;
                            
                            _logger.LogInformation("Rappel de séance envoyé pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                        else
                        {
                            _stats.FailedSends++;
                            _logger.LogWarning("Échec d'envoi du rappel de séance pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _stats.FailedSends++;
                        _logger.LogError(ex, "Erreur lors de l'envoi du rappel de séance pour la réservation {ReservationId}", reservation.ReservationId);
                    }
                }

                _stats.LastUpdate = DateTime.UtcNow;
                return sentCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des rappels de séance");
                throw;
            }
        }

        public async Task<int> SendExpirationWarningsAsync()
        {
            try
            {
                var reservations = await _unitOfWork.Reservations.GetAllAsync();
                var expiringReservations = reservations
                    .Where(r => r.Status == EnumConfig.ReservationStatus.Pending &&
                               r.CreatedAt.AddHours(22) <= DateTime.UtcNow.AddHours(2) &&
                               r.CreatedAt.AddHours(22) > DateTime.UtcNow)
                    .ToList();

                var sentCount = 0;

                foreach (var reservation in expiringReservations)
                {
                    try
                    {
                        var template = EmailTemplates.ExpirationWarningTemplate;
                        var emailBody = FormatEmailBody(template.Body, reservation);
                        
                        var success = await _emailService.SendEmailAsync(
                            reservation.AppUser?.Email ?? string.Empty,
                            template.Subject,
                            emailBody,
                            template.IsHtml
                        );

                        if (success)
                        {
                            sentCount++;
                            _stats.ExpirationWarningsSent++;
                            _stats.TotalSentToday++;
                            
                            _logger.LogInformation("Avertissement d'expiration envoyé pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                        else
                        {
                            _stats.FailedSends++;
                            _logger.LogWarning("Échec d'envoi de l'avertissement d'expiration pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _stats.FailedSends++;
                        _logger.LogError(ex, "Erreur lors de l'envoi de l'avertissement d'expiration pour la réservation {ReservationId}", reservation.ReservationId);
                    }
                }

                _stats.LastUpdate = DateTime.UtcNow;
                return sentCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des avertissements d'expiration");
                throw;
            }
        }

        public async Task<int> SendConfirmationNotificationsAsync()
        {
            try
            {
                var reservations = await _unitOfWork.Reservations.GetAllAsync();
                var confirmedReservations = reservations
                    .Where(r => r.Status == EnumConfig.ReservationStatus.Confirmed &&
                               r.UpdatedAt >= DateTime.UtcNow.AddHours(-1) &&
                               !r.ConfirmationSent)
                    .ToList();

                var sentCount = 0;

                foreach (var reservation in confirmedReservations)
                {
                    try
                    {
                        var template = EmailTemplates.ConfirmationTemplate;
                        var emailBody = FormatEmailBody(template.Body, reservation);
                        
                        var success = await _emailService.SendEmailAsync(
                            reservation.AppUser?.Email ?? string.Empty,
                            template.Subject,
                            emailBody,
                            template.IsHtml
                        );

                        if (success)
                        {
                            reservation.ConfirmationSent = true;
                            await _unitOfWork.Reservations.UpdateAsync(reservation);
                            sentCount++;
                            _stats.ConfirmationsSent++;
                            _stats.TotalSentToday++;
                            
                            _logger.LogInformation("Notification de confirmation envoyée pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                        else
                        {
                            _stats.FailedSends++;
                            _logger.LogWarning("Échec d'envoi de la notification de confirmation pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _stats.FailedSends++;
                        _logger.LogError(ex, "Erreur lors de l'envoi de la notification de confirmation pour la réservation {ReservationId}", reservation.ReservationId);
                    }
                }

                _stats.LastUpdate = DateTime.UtcNow;
                return sentCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des notifications de confirmation");
                throw;
            }
        }

        public async Task<int> SendCancellationNotificationsAsync()
        {
            try
            {
                var reservations = await _unitOfWork.Reservations.GetAllAsync();
                var cancelledReservations = reservations
                    .Where(r => r.Status == EnumConfig.ReservationStatus.Cancelled &&
                               r.UpdatedAt >= DateTime.UtcNow.AddHours(-1) &&
                               !r.CancellationSent)
                    .ToList();

                var sentCount = 0;

                foreach (var reservation in cancelledReservations)
                {
                    try
                    {
                        var template = EmailTemplates.CancellationTemplate;
                        var emailBody = FormatEmailBody(template.Body, reservation);
                        
                        // Remplacer le placeholder de raison d'annulation
                        emailBody = emailBody.Replace("{CancellationReason}", 
                            string.IsNullOrEmpty(reservation.CancellationReason) ? 
                            "Annulation par l'utilisateur" : reservation.CancellationReason);
                        
                        var success = await _emailService.SendEmailAsync(
                            reservation.AppUser?.Email ?? string.Empty,
                            template.Subject,
                            emailBody,
                            template.IsHtml
                        );

                        if (success)
                        {
                            reservation.CancellationSent = true;
                            await _unitOfWork.Reservations.UpdateAsync(reservation);
                            sentCount++;
                            _stats.CancellationsSent++;
                            _stats.TotalSentToday++;
                            
                            _logger.LogInformation("Notification d'annulation envoyée pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                        else
                        {
                            _stats.FailedSends++;
                            _logger.LogWarning("Échec d'envoi de la notification d'annulation pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _stats.FailedSends++;
                        _logger.LogError(ex, "Erreur lors de l'envoi de la notification d'annulation pour la réservation {ReservationId}", reservation.ReservationId);
                    }
                }

                _stats.LastUpdate = DateTime.UtcNow;
                return sentCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des notifications d'annulation");
                throw;
            }
        }

        public async Task<int> SendNoShowNotificationsAsync()
        {
            try
            {
                var reservations = await _unitOfWork.Reservations.GetAllAsync();
                var noShowReservations = reservations
                    .Where(r => r.Status == EnumConfig.ReservationStatus.NoShow &&
                               r.UpdatedAt >= DateTime.UtcNow.AddHours(-1) &&
                               !r.NoShowSent)
                    .ToList();

                var sentCount = 0;

                foreach (var reservation in noShowReservations)
                {
                    try
                    {
                        var template = EmailTemplates.NoShowTemplate;
                        var emailBody = FormatEmailBody(template.Body, reservation);
                        
                        var success = await _emailService.SendEmailAsync(
                            reservation.AppUser?.Email ?? string.Empty,
                            template.Subject,
                            emailBody,
                            template.IsHtml
                        );

                        if (success)
                        {
                            reservation.NoShowSent = true;
                            await _unitOfWork.Reservations.UpdateAsync(reservation);
                            sentCount++;
                            _stats.NoShowNotificationsSent++;
                            _stats.TotalSentToday++;
                            
                            _logger.LogInformation("Notification No Show envoyée pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                        else
                        {
                            _stats.FailedSends++;
                            _logger.LogWarning("Échec d'envoi de la notification No Show pour la réservation {ReservationId}", reservation.ReservationId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _stats.FailedSends++;
                        _logger.LogError(ex, "Erreur lors de l'envoi de la notification No Show pour la réservation {ReservationId}", reservation.ReservationId);
                    }
                }

                _stats.LastUpdate = DateTime.UtcNow;
                return sentCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des notifications No Show");
                throw;
            }
        }

        public async Task<List<Models.PostgresqlDb.Reservation>> GetReservationsNeedingPaymentRemindersAsync()
        {
            var reservations = await _unitOfWork.Reservations.GetAllAsync();
            return reservations
                .Where(r => r.Status == EnumConfig.ReservationStatus.Pending &&
                           r.CreatedAt <= DateTime.UtcNow.AddHours(-12) &&
                           (r.LastReminderSent == null || 
                            r.LastReminderSent <= DateTime.UtcNow.AddHours(-24)))
                .ToList();
        }

        public async Task<List<Models.PostgresqlDb.Reservation>> GetReservationsNeedingShowtimeRemindersAsync()
        {
            var reservations = await _unitOfWork.Reservations.GetAllAsync();
            return reservations
                .Where(r => r.Status == EnumConfig.ReservationStatus.Confirmed &&
                           r.Showtime != null &&
                           r.Showtime.StartTime <= DateTime.UtcNow.AddDays(1) &&
                           r.Showtime.StartTime > DateTime.UtcNow &&
                           (r.LastReminderSent == null || 
                            r.LastReminderSent <= DateTime.UtcNow.AddHours(-12)))
                .ToList();
        }

        public Task<ReminderStatsDto> GetReminderStatsAsync()
        {
            return Task.FromResult(_stats);
        }

        private string FormatEmailBody(string template, Models.PostgresqlDb.Reservation reservation)
        {
            var showtime = reservation.Showtime;
            var movie = showtime?.Movie;
            var theater = showtime?.Theater;
            var cinema = theater?.Cinema;
            var user = reservation.AppUser;

            var replacements = new Dictionary<string, string>
            {
                { "{UserName}", user?.FirstName ?? "Utilisateur" },
                { "{MovieTitle}", movie?.Title ?? "Film inconnu" },
                { "{ShowtimeDate}", showtime?.StartTime.ToString("dd/MM/yyyy") ?? "Date inconnue" },
                { "{ShowtimeTime}", showtime?.StartTime.ToString("HH:mm") ?? "Heure inconnue" },
                { "{TheaterName}", theater?.Name ?? "Salle inconnue" },
                { "{CinemaName}", cinema?.Name ?? "Cinéma inconnu" },
                { "{CinemaAddress}", cinema?.Address ?? "Adresse inconnue" },
                { "{TotalPrice}", reservation.TotalPrice.ToString("F2") },
                { "{PaymentDueDate}", reservation.CreatedAt.AddHours(24).ToString("dd/MM/yyyy HH:mm") },
                { "{NumberOfSeats}", reservation.Seats?.Count.ToString() ?? "0" }
            };

            var result = new StringBuilder(template);
            foreach (var replacement in replacements)
            {
                result.Replace(replacement.Key, replacement.Value);
            }

            return result.ToString();
        }
    }
}