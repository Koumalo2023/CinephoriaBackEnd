using CinephoriaServer.API.Models.PostgresqlDb.Settings;
using CinephoriaServer.API.Services.Notification;
using CinephoriaServer.API.Services.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CinephoriaServer.API.Models.MongooDb;
using static CinephoriaServer.API.Configurations.EnumConfig;
using NotificationTypeEnum = CinephoriaServer.API.Models.MongooDb.NotificationType;

namespace CinephoriaServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;

        public SettingsController(ISettingsService settingsService, INotificationService notificationService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        /// <summary>
        /// Récupère les paramètres généraux du système.
        /// </summary>
        /// <returns>Les paramètres généraux.</returns>
        [HttpGet("general")]
        [ProducesResponseType(typeof(GeneralSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGeneralSettings()
        {
            try
            {
                var settings = await _settingsService.GetGeneralSettingsAsync();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la récupération des paramètres généraux.", error = ex.Message });
            }
        }

        /// <summary>
        /// Met à jour les paramètres généraux du système (admin seulement).
        /// </summary>
        /// <param name="settingsDto">Les nouveaux paramètres généraux.</param>
        /// <returns>Résultat de l'opération.</returns>
        [HttpPut("general")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateGeneralSettings([FromBody] GeneralSettingsDto settingsDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _settingsService.UpdateGeneralSettingsAsync(settingsDto);
                return Ok(new { message = "Paramètres généraux mis à jour avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la mise à jour des paramètres généraux.", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère les paramètres de notifications du système.
        /// </summary>
        /// <returns>Les paramètres de notifications.</returns>
        [HttpGet("notifications")]
        [ProducesResponseType(typeof(NotificationSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotificationSettings()
        {
            try
            {
                var settings = await _settingsService.GetNotificationSettingsAsync();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la récupération des paramètres de notifications.", error = ex.Message });
            }
        }

        /// <summary>
        /// Met à jour les paramètres de notifications du système (admin seulement).
        /// </summary>
        /// <param name="settingsDto">Les nouveaux paramètres de notifications.</param>
        /// <returns>Résultat de l'opération.</returns>
        [HttpPut("notifications")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateNotificationSettings([FromBody] NotificationSettingsDto settingsDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _settingsService.UpdateNotificationSettingsAsync(settingsDto);
                return Ok(new { message = "Paramètres de notifications mis à jour avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la mise à jour des paramètres de notifications.", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère les paramètres de sécurité du système.
        /// </summary>
        /// <returns>Les paramètres de sécurité.</returns>
        [HttpGet("security")]
        [ProducesResponseType(typeof(SecuritySettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSecuritySettings()
        {
            try
            {
                var settings = await _settingsService.GetSecuritySettingsAsync();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la récupération des paramètres de sécurité.", error = ex.Message });
            }
        }

        /// <summary>
        /// Met à jour les paramètres de sécurité du système (admin seulement).
        /// </summary>
        /// <param name="settingsDto">Les nouveaux paramètres de sécurité.</param>
        /// <returns>Résultat de l'opération.</returns>
        [HttpPut("security")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSecuritySettings([FromBody] SecuritySettingsDto settingsDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _settingsService.UpdateSecuritySettingsAsync(settingsDto);
                return Ok(new { message = "Paramètres de sécurité mis à jour avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la mise à jour des paramètres de sécurité.", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère les notifications pour l'administration
        /// </summary>
        /// <param name="page">Numéro de page</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Liste des notifications</returns>
        [HttpGet("notifications/admin")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(typeof(List<NotificationModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAdminNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                // Cette méthode n'existe pas dans l'interface, à implémenter ou adapter
                var notifications = await _notificationService.GetUserNotificationsAsync("admin", pageSize, (page - 1) * pageSize);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la récupération des notifications admin.", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère les notifications d'un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="page">Numéro de page</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Liste des notifications</returns>
        [HttpGet("notifications/user/{userId}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(typeof(List<NotificationModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserNotifications(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var notifications = await _notificationService.GetUserNotificationsAsync(userId, pageSize, (page - 1) * pageSize);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la récupération des notifications utilisateur.", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère le nombre de notifications non lues pour un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <returns>Nombre de notifications non lues</returns>
        [HttpGet("notifications/unread-count/{userId}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUnreadNotificationCount(string userId)
        {
            try
            {
                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la récupération du nombre de notifications non lues.", error = ex.Message });
            }
        }

        /// <summary>
        /// Marque une notification comme lue
        /// </summary>
        /// <param name="notificationId">ID de la notification</param>
        /// <returns>Résultat de l'opération</returns>
        [HttpPut("notifications/mark-as-read/{notificationId}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(notificationId.ToString());
                return Ok(new { message = "Notification marquée comme lue avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors du marquage de la notification comme lue.", error = ex.Message });
            }
        }

        /// <summary>
        /// Marque toutes les notifications comme lues pour un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <returns>Résultat de l'opération</returns>
        [HttpPut("notifications/mark-all-as-read/{userId}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAllNotificationsAsRead(string userId)
        {
            try
            {
                await _notificationService.MarkAllAsReadAsync(userId);
                return Ok(new { message = "Toutes les notifications ont été marquées comme lues avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors du marquage de toutes les notifications comme lues.", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère les notifications par type
        /// </summary>
        /// <param name="type">Type de notification</param>
        /// <param name="page">Numéro de page</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Liste des notifications</returns>
        [HttpGet("notifications/type/{type}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [ProducesResponseType(typeof(List<NotificationModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotificationsByType(NotificationTypeEnum type, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                // Cette méthode n'existe pas dans l'interface, à implémenter ou adapter
                var allNotifications = await _notificationService.GetUserNotificationsAsync("all", 1000, 0);
                var filteredNotifications = allNotifications.Where(n => n.Type == type)
                                                          .Skip((page - 1) * pageSize)
                                                          .Take(pageSize)
                                                          .ToList();
                return Ok(filteredNotifications);
                return Ok(filteredNotifications);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erreur lors de la récupération des notifications par type.", error = ex.Message });
            }
        }
    }
}