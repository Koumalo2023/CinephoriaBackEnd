using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Models.PostgresqlDb.Settings;
using CinephoriaServer.API.Repository;
using System.Text.Json;

namespace CinephoriaServer.API.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingsService(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        }

        /// <summary>
        /// Récupère tous les paramètres généraux.
        /// </summary>
        /// <returns>DTO des paramètres généraux.</returns>
        public async Task<GeneralSettingsDto> GetGeneralSettingsAsync()
        {
            var settings = await _settingsRepository.GetSettingsByCategoryAsync("general");
            return MapToGeneralSettingsDto(settings);
        }

        /// <summary>
        /// Met à jour les paramètres généraux.
        /// </summary>
        /// <param name="settingsDto">DTO contenant les nouveaux paramètres.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateGeneralSettingsAsync(GeneralSettingsDto settingsDto)
        {
            var settingsToUpdate = new Dictionary<string, string>
            {
                { "general.companyName", settingsDto.CompanyName },
                { "general.companyAddress", settingsDto.CompanyAddress },
                { "general.contactEmail", settingsDto.ContactEmail },
                { "general.contactPhone", settingsDto.ContactPhone },
                { "general.language", settingsDto.Language },
                { "general.timezone", settingsDto.Timezone },
                { "general.dateFormat", settingsDto.DateFormat },
                { "general.currency", settingsDto.Currency }
            };

            await _settingsRepository.UpdateMultipleSettingsAsync(settingsToUpdate);
        }

        /// <summary>
        /// Récupère tous les paramètres de notifications.
        /// </summary>
        /// <returns>DTO des paramètres de notifications.</returns>
        public async Task<NotificationSettingsDto> GetNotificationSettingsAsync()
        {
            var settings = await _settingsRepository.GetSettingsByCategoryAsync("notifications");
            return MapToNotificationSettingsDto(settings);
        }

        /// <summary>
        /// Met à jour les paramètres de notifications.
        /// </summary>
        /// <param name="settingsDto">DTO contenant les nouveaux paramètres.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateNotificationSettingsAsync(NotificationSettingsDto settingsDto)
        {
            var settingsToUpdate = new Dictionary<string, string>
            {
                { "notifications.emailNewReservation", settingsDto.EmailNewReservation.ToString().ToLower() },
                { "notifications.emailCanceledReservation", settingsDto.EmailCanceledReservation.ToString().ToLower() },
                { "notifications.emailNewUser", settingsDto.EmailNewUser.ToString().ToLower() },
                { "notifications.emailSystemAlerts", settingsDto.EmailSystemAlerts.ToString().ToLower() },
                { "notifications.appNewReservation", settingsDto.AppNewReservation.ToString().ToLower() },
                { "notifications.appCanceledReservation", settingsDto.AppCanceledReservation.ToString().ToLower() },
                { "notifications.appNewUser", settingsDto.AppNewUser.ToString().ToLower() },
                { "notifications.appSystemAlerts", settingsDto.AppSystemAlerts.ToString().ToLower() },
                { "notifications.retention", settingsDto.Retention }
            };

            await _settingsRepository.UpdateMultipleSettingsAsync(settingsToUpdate);
        }

        /// <summary>
        /// Récupère tous les paramètres de sécurité.
        /// </summary>
        /// <returns>DTO des paramètres de sécurité.</returns>
        public async Task<SecuritySettingsDto> GetSecuritySettingsAsync()
        {
            var settings = await _settingsRepository.GetSettingsByCategoryAsync("security");
            return MapToSecuritySettingsDto(settings);
        }

        /// <summary>
        /// Met à jour les paramètres de sécurité.
        /// </summary>
        /// <param name="settingsDto">DTO contenant les nouveaux paramètres.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateSecuritySettingsAsync(SecuritySettingsDto settingsDto)
        {
            var settingsToUpdate = new Dictionary<string, string>
            {
                { "security.passwordMinLength", settingsDto.PasswordMinLength.ToString() },
                { "security.passwordRequireUppercase", settingsDto.PasswordRequireUppercase.ToString().ToLower() },
                { "security.passwordRequireNumber", settingsDto.PasswordRequireNumber.ToString().ToLower() },
                { "security.passwordRequireSpecial", settingsDto.PasswordRequireSpecial.ToString().ToLower() },
                { "security.passwordExpiry", settingsDto.PasswordExpiry }
            };

            await _settingsRepository.UpdateMultipleSettingsAsync(settingsToUpdate);
        }

        private GeneralSettingsDto MapToGeneralSettingsDto(List<Setting> settings)
        {
            var settingsDict = settings.ToDictionary(s => s.SettingKey, s => s.SettingValue);

            return new GeneralSettingsDto
            {
                CompanyName = settingsDict.GetValueOrDefault("general.companyName", "Cinéphoria"),
                CompanyAddress = settingsDict.GetValueOrDefault("general.companyAddress", ""),
                ContactEmail = settingsDict.GetValueOrDefault("general.contactEmail", "contact@cinephoria.com"),
                ContactPhone = settingsDict.GetValueOrDefault("general.contactPhone", ""),
                Language = settingsDict.GetValueOrDefault("general.language", "fr-FR"),
                Timezone = settingsDict.GetValueOrDefault("general.timezone", "Europe/Paris"),
                DateFormat = settingsDict.GetValueOrDefault("general.dateFormat", "dd/MM/yyyy"),
                Currency = settingsDict.GetValueOrDefault("general.currency", "EUR")
            };
        }

        private NotificationSettingsDto MapToNotificationSettingsDto(List<Setting> settings)
        {
            var settingsDict = settings.ToDictionary(s => s.SettingKey, s => s.SettingValue);

            return new NotificationSettingsDto
            {
                EmailNewReservation = bool.Parse(settingsDict.GetValueOrDefault("notifications.emailNewReservation", "true")),
                EmailCanceledReservation = bool.Parse(settingsDict.GetValueOrDefault("notifications.emailCanceledReservation", "true")),
                EmailNewUser = bool.Parse(settingsDict.GetValueOrDefault("notifications.emailNewUser", "true")),
                EmailSystemAlerts = bool.Parse(settingsDict.GetValueOrDefault("notifications.emailSystemAlerts", "true")),
                AppNewReservation = bool.Parse(settingsDict.GetValueOrDefault("notifications.appNewReservation", "true")),
                AppCanceledReservation = bool.Parse(settingsDict.GetValueOrDefault("notifications.appCanceledReservation", "true")),
                AppNewUser = bool.Parse(settingsDict.GetValueOrDefault("notifications.appNewUser", "true")),
                AppSystemAlerts = bool.Parse(settingsDict.GetValueOrDefault("notifications.appSystemAlerts", "true")),
                Retention = settingsDict.GetValueOrDefault("notifications.retention", "30")
            };
        }

        private SecuritySettingsDto MapToSecuritySettingsDto(List<Setting> settings)
        {
            var settingsDict = settings.ToDictionary(s => s.SettingKey, s => s.SettingValue);

            return new SecuritySettingsDto
            {
                PasswordMinLength = int.Parse(settingsDict.GetValueOrDefault("security.passwordMinLength", "8")),
                PasswordRequireUppercase = bool.Parse(settingsDict.GetValueOrDefault("security.passwordRequireUppercase", "true")),
                PasswordRequireNumber = bool.Parse(settingsDict.GetValueOrDefault("security.passwordRequireNumber", "true")),
                PasswordRequireSpecial = bool.Parse(settingsDict.GetValueOrDefault("security.passwordRequireSpecial", "true")),
                PasswordExpiry = settingsDict.GetValueOrDefault("security.passwordExpiry", "90")
            };
        }
    }
}