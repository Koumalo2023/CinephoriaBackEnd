using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinephoriaServer.API.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _adminDashboardRepository;

        public AdminDashboardService(IAdminDashboardRepository adminDashboardRepository)
        {
            _adminDashboardRepository = adminDashboardRepository;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            return await _adminDashboardRepository.GetDashboardStatsAsync();
        }

        public async Task<ReservationChartData> GetReservationChartDataAsync(string period)
        {
            return await _adminDashboardRepository.GetReservationChartDataAsync(period);
        }

        public async Task<List<TopFilm>> GetTopFilmsAsync()
        {
            return await _adminDashboardRepository.GetTopFilmsAsync();
        }

        public async Task<List<RecentReservation>> GetRecentReservationsAsync()
        {
            return await _adminDashboardRepository.GetRecentReservationsAsync();
        }

        public async Task<List<ActivityLog>> GetActivityLogsAsync()
        {
            return await _adminDashboardRepository.GetActivityLogsAsync();
        }

        // Méthodes pour les statistiques d'incidents
        public async Task<IncidentStats> GetIncidentStatsAsync()
        {
            return await _adminDashboardRepository.GetIncidentStatsAsync();
        }

        public async Task<IncidentChartData> GetIncidentChartDataAsync(string period)
        {
            return await _adminDashboardRepository.GetIncidentChartDataAsync(period);
        }

        public async Task<List<TopIncidentType>> GetTopIncidentTypesAsync()
        {
            return await _adminDashboardRepository.GetTopIncidentTypesAsync();
        }

        public async Task<List<IncidentDto>> GetRecentIncidentsAsync(int limit = 10)
        {
            return await _adminDashboardRepository.GetRecentIncidentsAsync(limit);
        }

        public async Task<List<IncidentActivity>> GetIncidentActivitiesAsync()
        {
            return await _adminDashboardRepository.GetIncidentActivitiesAsync();
        }
    }
}
