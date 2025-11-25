using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Repository
{
    public interface ISettingsRepository
    {
        /// <summary>
        /// Récupère tous les paramètres d'une catégorie spécifique.
        /// </summary>
        /// <param name="category">Catégorie des paramètres ('general', 'notifications', 'security').</param>
        /// <returns>Une liste de paramètres.</returns>
        Task<List<Setting>> GetSettingsByCategoryAsync(string category);

        /// <summary>
        /// Récupère un paramètre par sa clé.
        /// </summary>
        /// <param name="settingKey">Clé du paramètre.</param>
        /// <returns>Le paramètre correspondant.</returns>
        Task<Setting?> GetSettingByKeyAsync(string settingKey);

        /// <summary>
        /// Met à jour la valeur d'un paramètre.
        /// </summary>
        /// <param name="settingKey">Clé du paramètre.</param>
        /// <param name="settingValue">Nouvelle valeur du paramètre.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateSettingAsync(string settingKey, string settingValue);

        /// <summary>
        /// Met à jour plusieurs paramètres en une seule transaction.
        /// </summary>
        /// <param name="settings">Dictionnaire des paramètres à mettre à jour (clé, valeur).</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateMultipleSettingsAsync(Dictionary<string, string> settings);
    }
}