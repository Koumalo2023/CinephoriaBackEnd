using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository; 
using CinephoriaServer.API.Services.Settings;

namespace CinephoriaServer.API.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISettingsService _settingsService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IEmailService emailService,
            ISettingsService settingsService,
            INotificationRepository notificationRepository,
            IUserRepository userRepository,
            ILogger<NotificationService> logger)
        {
            _emailService = emailService;
            _settingsService = settingsService;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task SendNotificationAsync(NotificationType type, string recipientId, string? recipientEmail,
                                             string title, string message, Dictionary<string, object>? data = null)
        {
            // Créer et sauvegarder la notification
            var notification = new NotificationModel
            {
                Type = type,
                RecipientId = recipientId,
                RecipientEmail = recipientEmail,
                Title = title,
                Message = message,
                Data = data,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _notificationRepository.AddAsync(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout de la notification pour l'utilisateur {RecipientId}. La notification n'a pas été sauvegardée.", recipientId);
                // Ne pas propager l'exception pour éviter de bloquer l'application
            }

            // Récupérer les paramètres de notification
            var notificationSettings = await _settingsService.GetNotificationSettingsAsync();

            // Envoyer par email si l'email est fourni et selon les préférences
            if (!string.IsNullOrEmpty(recipientEmail))
            {
                bool shouldSendEmail = type switch
                {
                    NotificationType.EmailNewReservation => notificationSettings.EmailNewReservation,
                    NotificationType.EmailCanceledReservation => notificationSettings.EmailCanceledReservation,
                    NotificationType.EmailNewUser => notificationSettings.EmailNewUser,
                    NotificationType.EmailSystemAlerts => notificationSettings.EmailSystemAlerts,
                    _ => false
                };

                if (shouldSendEmail)
                {
                    await _emailService.SendEmailAsync(recipientEmail, title, message, true);
                }
            }

            // Envoyer une notification push si applicable
            bool shouldSendPush = type switch
            {
                NotificationType.AppNewReservation => notificationSettings.AppNewReservation,
                NotificationType.AppCanceledReservation => notificationSettings.AppCanceledReservation,
                NotificationType.AppNewUser => notificationSettings.AppNewUser,
                NotificationType.AppSystemAlerts => notificationSettings.AppSystemAlerts,
                _ => false
            };

            if (shouldSendPush && !string.IsNullOrEmpty(recipientId))
            {
                await SendPushNotificationAsync(recipientId, title, message);
            }
        }

        public async Task SendEmailNotificationAsync(NotificationType type, string recipientEmail,
                                                  string subject, string message, Dictionary<string, object>? data = null)
        {
            var notificationSettings = await _settingsService.GetNotificationSettingsAsync();
            bool shouldSendEmail = type switch
            {
                NotificationType.EmailNewReservation => notificationSettings.EmailNewReservation,
                NotificationType.EmailCanceledReservation => notificationSettings.EmailCanceledReservation,
                NotificationType.EmailNewUser => notificationSettings.EmailNewUser,
                NotificationType.EmailSystemAlerts => notificationSettings.EmailSystemAlerts,
                _ => false
            };

            if (shouldSendEmail)
            {
                await _emailService.SendEmailAsync(recipientEmail, subject, message, true);
            }
        }

        public async Task SendAppNotificationAsync(NotificationType type, string recipientId,
                                                string title, string message, Dictionary<string, object>? data = null)
        {
            var notificationSettings = await _settingsService.GetNotificationSettingsAsync();
            bool shouldSendPush = type switch
            {
                NotificationType.AppNewReservation => notificationSettings.AppNewReservation,
                NotificationType.AppCanceledReservation => notificationSettings.AppCanceledReservation,
                NotificationType.AppNewUser => notificationSettings.AppNewUser,
                NotificationType.AppSystemAlerts => notificationSettings.AppSystemAlerts,
                _ => false
            };

            if (shouldSendPush)
            {
                await SendPushNotificationAsync(recipientId, title, message);
                
                // Sauvegarder la notification
                var notification = new NotificationModel
                {
                    Type = type,
                    RecipientId = recipientId,
                    Title = title,
                    Message = message,
                    Data = data,
                    IsRead = false,
                    SentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    await _notificationRepository.AddAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de l'ajout de la notification d'application pour l'utilisateur {RecipientId}. La notification n'a pas été sauvegardée.", recipientId);
                    // Ne pas propager l'exception pour éviter de bloquer l'application
                }
            }
        }

        public async Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(string userId, int limit = 50, int skip = 0)
        {
            try
            {
                return await _notificationRepository.GetUserNotificationsAsync(userId, limit, skip);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des notifications pour l'utilisateur {UserId}. Retour de notifications vides.", userId);
                // Retourner une liste vide en cas d'erreur pour éviter de bloquer l'application
                return Enumerable.Empty<NotificationModel>();
            }
        }

        public async Task MarkAsReadAsync(string notificationId)
        {
            try
            {
                await _notificationRepository.MarkAsReadAsync(notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage de la notification {NotificationId} comme lue", notificationId);
                // Ne pas propager l'exception pour éviter de bloquer l'application
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            try
            {
                await _notificationRepository.MarkAllAsReadAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage de toutes les notifications comme lues pour l'utilisateur {UserId}", userId);
                // Ne pas propager l'exception pour éviter de bloquer l'application
            }
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            try
            {
                return await _notificationRepository.GetUnreadCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du nombre de notifications non lues pour l'utilisateur {UserId}. Retour de 0.", userId);
                // Retourner 0 en cas d'erreur pour éviter de bloquer l'application
                return 0;
            }
        }

        // Méthodes utilitaires privées
        private async Task<UserProfileDto> GetUserDetailsAsync(string userId)
        {
            // Implémentation pour récupérer les détails de l'utilisateur
            // Cette méthode devrait être implémentée en utilisant UserRepository ou UserService
            return null; // Placeholder
        }

        private async Task<List<string>> GetAdminEmailsAsync()
        {
            // Récupérer les emails des administrateurs
            return new List<string>(); // Placeholder
        }

        private async Task SendPushNotificationAsync(string userId, string title, string message)
        {
            // Implémentation pour les notifications push (Firebase, OneSignal, etc.)
            _logger.LogInformation("Notification push envoyée à {UserId}: {Title} - {Message}", userId, title, message);
        }

        private async Task SendPushNotificationToAdminsAsync(string title, string message)
        {
            // Envoyer une notification push à tous les administrateurs
            _logger.LogInformation("Notification push admin: {Title} - {Message}", title, message);
        }

        // Méthodes spécifiques pour les différents types de notifications (facilitateurs)
        public async Task SendNewReservationNotificationAsync(string userId, ReservationDto reservationDetails)
        {
            var user = await GetUserDetailsAsync(userId);
            if (user != null)
            {
                var title = "Confirmation de votre réservation Cinephoria";
                var message = GenerateNewReservationEmailBody(user, reservationDetails);
                
                await SendNotificationAsync(NotificationType.EmailNewReservation, userId, user.Email, 
                                         title, message, new Dictionary<string, object>
                                         {
                                             {"reservationId", reservationDetails.ReservationId},
                                             {"numberOfSeats", reservationDetails.NumberOfSeats},
                                             {"totalPrice", reservationDetails.TotalPrice}
                                         });

                await SendNotificationAsync(NotificationType.AppNewReservation, userId, null,
                                         "Nouvelle réservation", "Votre réservation a été confirmée avec succès.",
                                         new Dictionary<string, object>
                                         {
                                             {"reservationId", reservationDetails.ReservationId}
                                         });
            }
        }

        public async Task SendCanceledReservationNotificationAsync(string userId, ReservationDto reservationDetails)
        {
            var user = await GetUserDetailsAsync(userId);
            if (user != null)
            {
                var title = "Annulation de votre réservation Cinephoria";
                var message = GenerateCanceledReservationEmailBody(user, reservationDetails);
                
                await SendNotificationAsync(NotificationType.EmailCanceledReservation, userId, user.Email, 
                                         title, message, new Dictionary<string, object>
                                         {
                                             {"reservationId", reservationDetails.ReservationId}
                                         });

                await SendNotificationAsync(NotificationType.AppCanceledReservation, userId, null,
                                         "Réservation annulée", "Votre réservation a été annulée.",
                                         new Dictionary<string, object>
                                         {
                                             {"reservationId", reservationDetails.ReservationId}
                                         });
            }
        }

        public async Task SendNewUserNotificationAsync(string userId, UserProfileDto userDetails)
        {
            var title = "Bienvenue sur Cinephoria !";
            var message = GenerateNewUserEmailBody(userDetails);
            
            await SendNotificationAsync(NotificationType.EmailNewUser, userId, userDetails.Email, 
                                     title, message);

            await SendNotificationAsync(NotificationType.AppNewUser, userId, null,
                                     "Bienvenue !", "Votre compte Cinephoria a été créé avec succès.");
        }

        public async Task SendSystemAlertNotificationAsync(string message, string severity = "info")
        {
            var adminEmails = await GetAdminEmailsAsync();
            var title = $"Alerte système Cinephoria - {severity.ToUpper()}";
            var fullMessage = GenerateSystemAlertEmailBody(message, severity);

            foreach (var email in adminEmails)
            {
                await SendNotificationAsync(NotificationType.EmailSystemAlerts, "admin", email, 
                                         title, fullMessage, new Dictionary<string, object>
                                         {
                                             {"severity", severity}
                                         });
            }

            await SendNotificationAsync(NotificationType.AppSystemAlerts, "admin", null,
                                     "Alerte système", message, new Dictionary<string, object>
                                     {
                                         {"severity", severity}
                                     });
        }

        // Méthodes de génération de templates d'email
        private string GenerateNewReservationEmailBody(UserProfileDto user, ReservationDto reservation)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Confirmation de réservation</title>
</head>
<body>
    <h2>Bonjour {user.FirstName} {user.LastName},</h2>
    <p>Votre réservation a été confirmée avec succès.</p>
    <p><strong>Détails de la réservation :</strong></p>
    <ul>
        <li>Référence : {reservation.ReservationId}</li>
        <li>Nombre de places : {reservation.NumberOfSeats}</li>
        <li>Prix total : {reservation.TotalPrice} €</li>
    </ul>
    <p>Merci de votre confiance !</p>
    <p>L'équipe Cinephoria</p>
</body>
</html>";
        }

        private string GenerateCanceledReservationEmailBody(UserProfileDto user, ReservationDto reservation)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Annulation de réservation</title>
</head>
<body>
    <h2>Bonjour {user.FirstName} {user.LastName},</h2>
    <p>Votre réservation n°{reservation.ReservationId} a été annulée.</p>
    <p>Si vous n'êtes pas à l'origine de cette annulation, veuillez contacter le support.</p>
    <p>L'équipe Cinephoria</p>
</body>
</html>";
        }

        private string GenerateNewUserEmailBody(UserProfileDto user)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Bienvenue sur Cinephoria</title>
</head>
<body>
    <h2>Bienvenue {user.FirstName} {user.LastName} !</h2>
    <p>Votre compte a été créé avec succès sur Cinephoria.</p>
    <p>Vous pouvez dès maintenant réserver vos places de cinéma en ligne.</p>
    <p>L'équipe Cinephoria</p>
</body>
</html>";
        }

        private string GenerateSystemAlertEmailBody(string message, string severity)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Alerte système</title>
</head>
<body>
    <h2>Alerte système - {severity.ToUpper()}</h2>
    <p>{message}</p>
    <p>Date : {DateTime.Now}</p>
</body>
</html>";
        }
    }
}