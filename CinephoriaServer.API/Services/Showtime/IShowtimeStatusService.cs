using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.DTOs.Showtime;

namespace CinephoriaServer.API.Services
{
    public interface IShowtimeStatusService
    {
        /// <summary>
        /// Met à jour automatiquement les statuts des séances en fonction de l'heure actuelle
        /// </summary>
        Task<int> UpdateAllShowtimeStatusesAsync();

        /// <summary>
        /// Récupère les séances par statut
        /// </summary>
        /// <param name="status">Statut des séances</param>
        /// <returns>Liste des séances avec le statut spécifié</returns>
        Task<List<ShowtimeStatusDto>> GetShowtimesByStatusAsync(EnumConfig.ShowtimeStatus status);

        /// <summary>
        /// Récupère toutes les séances avec leurs statuts
        /// </summary>
        /// <returns>Liste de toutes les séances avec statuts</returns>
        Task<List<ShowtimeStatusDto>> GetAllShowtimesWithStatusAsync();

        /// <summary>
        /// Récupère les séances à venir (prochaines 24h)
        /// </summary>
        /// <returns>Liste des séances à venir</returns>
        Task<List<ShowtimeStatusDto>> GetUpcomingShowtimesAsync();

        /// <summary>
        /// Récupère les séances en cours
        /// </summary>
        /// <returns>Liste des séances en cours</returns>
        Task<List<ShowtimeStatusDto>> GetOngoingShowtimesAsync();

        /// <summary>
        /// Met à jour le statut d'une séance spécifique
        /// </summary>
        /// <param name="showtimeId">ID de la séance</param>
        /// <param name="newStatus">Nouveau statut</param>
        /// <returns>True si la mise à jour a réussi</returns>
        Task<bool> UpdateShowtimeStatusAsync(int showtimeId, EnumConfig.ShowtimeStatus newStatus);

        /// <summary>
        /// Récupère les statistiques des statuts des séances
        /// </summary>
        /// <returns>Statistiques des séances</returns>
        Task<ShowtimeStatusStatsDto> GetShowtimeStatsAsync();

        /// <summary>
        /// Récupère les films ayant au moins une séance
        /// </summary>
        /// <returns>Liste des films avec séances</returns>
        Task<List<object>> GetMoviesWithShowtimesAsync();

        /// <summary>
        /// Récupère les films ajoutés le dernier mercredi avec leurs séances
        /// </summary>
        /// <returns>Liste des films récents avec séances</returns>
        Task<List<object>> GetRecentMoviesWithShowtimesAsync();
        /// <summary>
        /// Met à jour les statuts des séances (alias pour UpdateAllShowtimeStatusesAsync)
        /// </summary>
        Task<int> UpdateShowtimeStatusesAsync();
    }
}