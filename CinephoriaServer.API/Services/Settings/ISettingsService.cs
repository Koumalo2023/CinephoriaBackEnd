using CinephoriaServer.API.Models.PostgresqlDb.Settings;

namespace CinephoriaServer.API.Services.Settings
{
    public interface ISettingsService
    {
        /// <summary>
        /// Récupère tous les paramètres généraux.
        /// </summary>
        /// <returns>DTO des paramètres généraux.</returns>
        Task<GeneralSettingsDto> GetGeneralSettingsAsync();

        /// <summary>
        /// Met à jour les paramètres généraux.
        /// </summary>
        /// <param name="settingsDto">DTO contenant les nouveaux paramètres.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateGeneralSettingsAsync(GeneralSettingsDto settingsDto);

        /// <summary>
        /// Récupère tous les paramètres de notifications.
        /// </summary>
        /// <returns>DTO des paramètres de notifications.</returns>
        Task<NotificationSettingsDto> GetNotificationSettingsAsync();

        /// <summary>
        /// Met à jour les paramètres de notifications.
        /// </summary>
        /// <param name="settingsDto">DTO contenant les nouveaux paramètres.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateNotificationSettingsAsync(NotificationSettingsDto settingsDto);

        /// <summary>
        /// Récupère tous les paramètres de sécurité.
        /// </summary>
        /// <returns>DTO des paramètres de sécurité.</returns>
        Task<SecuritySettingsDto> GetSecuritySettingsAsync();

        /// <summary>
        /// Met à jour les paramètres de sécurité.
        /// </summary>
        /// <param name="settingsDto">DTO contenant les nouveaux paramètres.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateSecuritySettingsAsync(SecuritySettingsDto settingsDto);
    }
}