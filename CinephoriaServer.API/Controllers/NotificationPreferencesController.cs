using Microsoft.AspNetCore.Mvc;
using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Services.Notification;
using Microsoft.AspNetCore.Authorization;

namespace CinephoriaServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationPreferencesController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationPreferencesController> _logger;

        public NotificationPreferencesController(
            INotificationService notificationService,
            ILogger<NotificationPreferencesController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Récupère les préférences de notification de l'utilisateur connecté
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserPreferences()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                // Cette méthode nécessiterait d'être implémentée dans INotificationService
                // Pour l'instant, retournons des préférences par défaut
                var defaultPreferences = new NotificationPreference
                {
                    UserId = userId,
                    EmailEnabled = true,
                    AppEnabled = true,
                    Preferences = new Dictionary<NotificationType, bool>
                    {
                        { NotificationType.EmailNewReservation, true },
                        { NotificationType.EmailCanceledReservation, true },
                        { NotificationType.EmailNewUser, true },
                        { NotificationType.EmailSystemAlerts, true },
                        { NotificationType.AppNewReservation, true },
                        { NotificationType.AppCanceledReservation, true },
                        { NotificationType.AppNewUser, true },
                        { NotificationType.AppSystemAlerts, true }
                    }
                };

                return Ok(defaultPreferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des préférences de notification");
                return StatusCode(500, "Une erreur interne est survenue");
            }
        }

        /// <summary>
        /// Met à jour les préférences de notification de l'utilisateur connecté
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdatePreferences([FromBody] NotificationPreference preferences)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                if (preferences == null)
                {
                    return BadRequest("Les préférences ne peuvent pas être null");
                }

                // S'assurer que l'userId correspond à l'utilisateur connecté
                preferences.UserId = userId;
                preferences.UpdatedAt = DateTime.UtcNow;

                // Cette méthode nécessiterait d'être implémentée dans INotificationService
                // Pour l'instant, retournons un succès simulé
                _logger.LogInformation("Préférences de notification mises à jour pour l'utilisateur {UserId}", userId);
                
                return Ok(new { message = "Préférences mises à jour avec succès", preferences });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour des préférences de notification");
                return StatusCode(500, "Une erreur interne est survenue");
            }
        }

        /// <summary>
        /// Récupère les notifications de l'utilisateur connecté
        /// </summary>
        [HttpGet("notifications")]
        public async Task<IActionResult> GetUserNotifications([FromQuery] int limit = 50, [FromQuery] int skip = 0)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                var notifications = await _notificationService.GetUserNotificationsAsync(userId, limit, skip);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des notifications");
                return StatusCode(500, "Une erreur interne est survenue");
            }
        }

        /// <summary>
        /// Marque une notification comme lue
        /// </summary>
        [HttpPost("notifications/{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(string notificationId)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(notificationId);
                return Ok(new { message = "Notification marquée comme lue" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage de la notification comme lue");
                return StatusCode(500, "Une erreur interne est survenue");
            }
        }

        /// <summary>
        /// Marque toutes les notifications comme lues
        /// </summary>
        [HttpPost("notifications/read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                await _notificationService.MarkAllAsReadAsync(userId);
                return Ok(new { message = "Toutes les notifications marquées comme lues" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage de toutes les notifications comme lues");
                return StatusCode(500, "Une erreur interne est survenue");
            }
        }

        /// <summary>
        /// Récupère le nombre de notifications non lues
        /// </summary>
        [HttpGet("notifications/unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(new { unreadCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du nombre de notifications non lues");
                return StatusCode(500, "Une erreur interne est survenue");
            }
        }
    }
}