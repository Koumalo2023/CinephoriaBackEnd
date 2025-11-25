using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Repository
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly CinephoriaDbContext _context;

        public SettingsRepository(CinephoriaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Récupère tous les paramètres d'une catégorie spécifique.
        /// </summary>
        /// <param name="category">Catégorie des paramètres ('general', 'notifications', 'security').</param>
        /// <returns>Une liste de paramètres.</returns>
        public async Task<List<Setting>> GetSettingsByCategoryAsync(string category)
        {
            return await _context.Settings
                .Where(s => s.Category == category)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère un paramètre par sa clé.
        /// </summary>
        /// <param name="settingKey">Clé du paramètre.</param>
        /// <returns>Le paramètre correspondant.</returns>
        public async Task<Setting?> GetSettingByKeyAsync(string settingKey)
        {
            return await _context.Settings
                .FirstOrDefaultAsync(s => s.SettingKey == settingKey);
        }

        /// <summary>
        /// Met à jour la valeur d'un paramètre.
        /// </summary>
        /// <param name="settingKey">Clé du paramètre.</param>
        /// <param name="settingValue">Nouvelle valeur du paramètre.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateSettingAsync(string settingKey, string settingValue)
        {
            var setting = await _context.Settings
                .FirstOrDefaultAsync(s => s.SettingKey == settingKey);

            if (setting != null)
            {
                setting.SettingValue = settingValue;
                setting.UpdatedAt = DateTime.UtcNow;
                _context.Settings.Update(setting);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Met à jour plusieurs paramètres en une seule transaction.
        /// </summary>
        /// <param name="settings">Dictionnaire des paramètres à mettre à jour (clé, valeur).</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateMultipleSettingsAsync(Dictionary<string, string> settings)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                foreach (var (settingKey, settingValue) in settings)
                {
                    var setting = await _context.Settings
                        .FirstOrDefaultAsync(s => s.SettingKey == settingKey);

                    if (setting != null)
                    {
                        setting.SettingValue = settingValue;
                        setting.UpdatedAt = DateTime.UtcNow;
                        _context.Settings.Update(setting);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}