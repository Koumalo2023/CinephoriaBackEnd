using CinephoriaServer.API.Models.MongooDb;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinephoriaServer.API.Services
{
    public interface IAdminDashboardService
    {
        /// <summary>
        /// Récupère les statistiques globales pour le tableau de bord
        /// </summary>
        Task<DashboardStats> GetDashboardStatsAsync();

        /// <summary>
        /// Récupère les données pour le graphique des réservations par période
        /// </summary>
        /// <param name="period">Période: "week", "month" ou "year"</param>
        Task<ReservationChartData> GetReservationChartDataAsync(string period);

        /// <summary>
        /// Récupère la liste des films les plus populaires
        /// </summary>
        Task<List<TopFilm>> GetTopFilmsAsync();

        /// <summary>
        /// Récupère la liste des réservations les plus récentes
        /// </summary>
        Task<List<RecentReservation>> GetRecentReservationsAsync();

        /// <summary>
        /// Récupère le journal des activités récentes du système
        /// </summary>
        Task<List<ActivityLog>> GetActivityLogsAsync();

        // Méthodes pour les statistiques d'incidents
        /// <summary>
        /// Récupère les statistiques globales des incidents
        /// </summary>
        Task<IncidentStats> GetIncidentStatsAsync();

        /// <summary>
        /// Récupère les données pour le graphique d'évolution des incidents
        /// </summary>
        /// <param name="period">Période: "week", "month" ou "year"</param>
        Task<IncidentChartData> GetIncidentChartDataAsync(string period);

        /// <summary>
        /// Récupère les types d'incidents les plus fréquents
        /// </summary>
        Task<List<TopIncidentType>> GetTopIncidentTypesAsync();

        /// <summary>
        /// Récupère les incidents récents
        /// </summary>
        /// <param name="limit">Nombre maximum d'incidents à récupérer</param>
        Task<List<IncidentDto>> GetRecentIncidentsAsync(int limit = 10);

        /// <summary>
        /// Récupère le journal des activités liées aux incidents
        /// </summary>
        Task<List<IncidentActivity>> GetIncidentActivitiesAsync();
    }
}
