using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinephoriaServer.API.Controllers
{
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        /// <summary>
        /// Récupère les statistiques globales pour le tableau de bord
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _adminDashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Récupère les données pour le graphique des réservations par période
        /// </summary>
        /// <param name="period">Période: "week", "month" ou "year"</param>
        [HttpGet("reservations-chart")]
        public async Task<IActionResult> GetReservationChartData([FromQuery] string period)
        {
            if (string.IsNullOrEmpty(period) || !(period.ToLower() == "week" || period.ToLower() == "month" || period.ToLower() == "year"))
            {
                return BadRequest("Le paramètre 'period' est requis et doit être 'week', 'month' ou 'year'");
            }

            var chartData = await _adminDashboardService.GetReservationChartDataAsync(period);
            return Ok(chartData);
        }

        /// <summary>
        /// Récupère la liste des films les plus populaires
        /// </summary>
        [HttpGet("top-films")]
        public async Task<IActionResult> GetTopFilms()
        {
            var topFilms = await _adminDashboardService.GetTopFilmsAsync();
            return Ok(topFilms);
        }

        /// <summary>
        /// Récupère la liste des réservations les plus récentes
        /// </summary>
        [HttpGet("recent-reservations")]
        public async Task<IActionResult> GetRecentReservations()
        {
            var recentReservations = await _adminDashboardService.GetRecentReservationsAsync();
            return Ok(recentReservations);
        }

        /// <summary>
        /// Récupère le journal des activités récentes du système
        /// </summary>
        [HttpGet("activities")]
        public async Task<IActionResult> GetActivityLogs()
        {
            var activities = await _adminDashboardService.GetActivityLogsAsync();
            return Ok(activities);
        }

        // Endpoints pour les statistiques d'incidents
        /// <summary>
        /// Récupère les statistiques globales des incidents
        /// </summary>
        [HttpGet("incidents/stats")]
        public async Task<IActionResult> GetIncidentStats()
        {
            var stats = await _adminDashboardService.GetIncidentStatsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Récupère les données pour le graphique d'évolution des incidents
        /// </summary>
        /// <param name="period">Période: "week", "month" ou "year"</param>
        [HttpGet("incidents/chart")]
        public async Task<IActionResult> GetIncidentChartData([FromQuery] string period)
        {
            if (string.IsNullOrEmpty(period) || !(period.ToLower() == "week" || period.ToLower() == "month" || period.ToLower() == "year"))
            {
                return BadRequest("Le paramètre 'period' est requis et doit être 'week', 'month' ou 'year'");
            }

            var chartData = await _adminDashboardService.GetIncidentChartDataAsync(period);
            return Ok(chartData);
        }

        /// <summary>
        /// Récupère les types d'incidents les plus fréquents
        /// </summary>
        [HttpGet("incidents/top-types")]
        public async Task<IActionResult> GetTopIncidentTypes()
        {
            var topTypes = await _adminDashboardService.GetTopIncidentTypesAsync();
            return Ok(topTypes);
        }

        /// <summary>
        /// Récupère les incidents récents
        /// </summary>
        /// <param name="limit">Nombre maximum d'incidents à récupérer (optionnel, défaut: 10)</param>
        [HttpGet("incidents/recent")]
        public async Task<IActionResult> GetRecentIncidents([FromQuery] int limit = 10)
        {
            var recentIncidents = await _adminDashboardService.GetRecentIncidentsAsync(limit);
            return Ok(recentIncidents);
        }

        /// <summary>
        /// Récupère le journal des activités liées aux incidents
        /// </summary>
        [HttpGet("incidents/activities")]
        public async Task<IActionResult> GetIncidentActivities()
        {
            var activities = await _adminDashboardService.GetIncidentActivitiesAsync();
            return Ok(activities);
        }
    }
}
