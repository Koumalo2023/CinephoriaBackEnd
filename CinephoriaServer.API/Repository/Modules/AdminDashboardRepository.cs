using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Repository
{
    public interface IAdminDashboardRepository
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<ReservationChartData> GetReservationChartDataAsync(string period);
        Task<List<TopFilm>> GetTopFilmsAsync();
        Task<List<RecentReservation>> GetRecentReservationsAsync();
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
        Task<List<CinephoriaServer.API.Models.MongooDb.IncidentDto>> GetRecentIncidentsAsync(int limit = 10);

        /// <summary>
        /// Récupère le journal des activités liées aux incidents
        /// </summary>
        Task<List<IncidentActivity>> GetIncidentActivitiesAsync();
    }

    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly CinephoriaDbContext _context;

        public AdminDashboardRepository(CinephoriaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            // Récupérer les réservations d'aujourd'hui
            var reservationsToday = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt.Date == today && r.Status == ReservationStatus.Confirmed)
                .CountAsync();

            // Récupérer les réservations d'hier pour calculer la tendance
            var reservationsYesterday = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt.Date == yesterday && r.Status == ReservationStatus.Confirmed)
                .CountAsync();

            // Calculer la tendance des réservations
            var reservationsTrend = reservationsYesterday > 0
                ? (int)((reservationsToday - reservationsYesterday) * 100 / reservationsYesterday)
                : 100;

            // Récupérer le revenu d'aujourd'hui
            var revenueToday = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt.Date == today && r.Status == ReservationStatus.Confirmed)
                .SumAsync(r => (decimal?)r.TotalPrice) ?? 0;

            // Récupérer le revenu d'hier pour calculer la tendance
            var revenueYesterday = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt.Date == yesterday && r.Status == ReservationStatus.Confirmed)
                .SumAsync(r => (decimal?)r.TotalPrice) ?? 0;

            // Calculer la tendance du revenu
            var revenueTrend = revenueYesterday > 0
                ? (int)((revenueToday - revenueYesterday) * 100 / revenueYesterday)
                : 100;

            // Récupérer le nombre de visiteurs uniques aujourd'hui
            var visitorsToday = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt.Date == today && r.Status == ReservationStatus.Confirmed)
                .Select(r => r.AppUserId)
                .Distinct()
                .CountAsync();

            // Récupérer le nombre de visiteurs uniques hier pour calculer la tendance
            var visitorsYesterday = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt.Date == yesterday && r.Status == ReservationStatus.Confirmed)
                .Select(r => r.AppUserId)
                .Distinct()
                .CountAsync();

            // Calculer la tendance des visiteurs
            var visitorsTrend = visitorsYesterday > 0
                ? (int)((visitorsToday - visitorsYesterday) * 100 / visitorsYesterday)
                : 100;

            // Calculer le taux d'occupation moyen aujourd'hui
            var occupancyRate = await CalculateOccupancyRateAsync(today);

            // Calculer le taux d'occupation hier pour la tendance
            var occupancyRateYesterday = await CalculateOccupancyRateAsync(yesterday);
            var occupancyTrend = occupancyRateYesterday > 0
                ? (int)((occupancyRate - occupancyRateYesterday) * 100 / occupancyRateYesterday)
                : 100;

            return new DashboardStats
            {
                ReservationsToday = reservationsToday,
                ReservationsTrend = reservationsTrend,
                RevenueToday = revenueToday,
                RevenueTrend = revenueTrend,
                VisitorsToday = visitorsToday,
                VisitorsTrend = visitorsTrend,
                OccupancyRate = occupancyRate,
                OccupancyTrend = occupancyTrend,
                Timestamp = DateTime.UtcNow
            };
        }

        private async Task<int> CalculateOccupancyRateAsync(DateTime date)
        {
            // Récupérer toutes les séances du jour
            var showtimes = await _context.Set<Showtime>()
                .Where(s => s.StartTime.Date == date)
                .Include(s => s.Theater)
                .ToListAsync();

            if (!showtimes.Any())
                return 0;

            // Calculer le taux d'occupation moyen
            var totalCapacity = showtimes.Sum(s => s.Theater?.Seats?.Count ?? 0);
            var totalReservedSeats = await _context.Set<Reservation>()
                .Where(r => r.Showtime.StartTime.Date == date && r.Status == ReservationStatus.Confirmed)
                .SumAsync(r => r.NumberOfSeats);

            if (totalCapacity == 0)
                return 0;

            return (int)(totalReservedSeats * 100 / totalCapacity);
        }

        public async Task<ReservationChartData> GetReservationChartDataAsync(string period)
        {
            var data = new List<ReservationChartItem>();
            var today = DateTime.UtcNow.Date;

            switch (period.ToLower())
            {
                case "week":
                    for (int i = 6; i >= 0; i--)
                    {
                        var date = today.AddDays(-i);
                        var dayData = await GetReservationDataForDateAsync(date);
                        data.Add(dayData);
                    }
                    break;
                case "month":
                    for (int i = 29; i >= 0; i--)
                    {
                        var date = today.AddDays(-i);
                        var dayData = await GetReservationDataForDateAsync(date);
                        data.Add(dayData);
                    }
                    break;
                case "year":
                    for (int i = 11; i >= 0; i--)
                    {
                        var date = today.AddMonths(-i);
                        var monthData = await GetReservationDataForMonthAsync(date.Year, date.Month);
                        data.Add(monthData);
                    }
                    break;
            }

            return new ReservationChartData
            {
                Period = period,
                Data = data
            };
        }

        private async Task<ReservationChartItem> GetReservationDataForDateAsync(DateTime date)
        {
            var reservations = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt.Date == date && r.Status == ReservationStatus.Confirmed)
                .ToListAsync();

            var totalReservations = reservations.Count;
            var totalRevenue = reservations.Sum(r => (decimal)r.TotalPrice);

            return new ReservationChartItem
            {
                Date = date,
                Reservations = totalReservations,
                Revenue = totalRevenue
            };
        }

        private async Task<ReservationChartItem> GetReservationDataForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var reservations = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt.Date >= startDate && r.CreatedAt.Date <= endDate && r.Status == ReservationStatus.Confirmed)
                .ToListAsync();

            var totalReservations = reservations.Count;
            var totalRevenue = reservations.Sum(r => (decimal)r.TotalPrice);

            return new ReservationChartItem
            {
                Date = startDate,
                Reservations = totalReservations,
                Revenue = totalRevenue
            };
        }

        public async Task<List<TopFilm>> GetTopFilmsAsync()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);

            var topFilms = await _context.Set<Reservation>()
                .Where(r => r.CreatedAt >= last30Days && r.Status == ReservationStatus.Confirmed)
                .GroupBy(r => r.Showtime.Movie)
                .Select(g => new
                {
                    Movie = g.Key,
                    Reservations = g.Count(),
                    Revenue = g.Sum(r => (decimal)r.TotalPrice),
                    TotalSeats = g.Sum(r => r.NumberOfSeats)
                })
                .OrderByDescending(x => x.Reservations)
                .Take(10)
                .ToListAsync();

            var result = new List<TopFilm>();

            foreach (var filmData in topFilms)
            {
                // Calculer le taux d'occupation
                var totalShowtimes = await _context.Set<Showtime>()
                    .Where(s => s.MovieId == filmData.Movie.MovieId && s.StartTime >= last30Days)
                    .Include(s => s.Theater)
                    .ToListAsync();

                var totalCapacity = totalShowtimes.Sum(s => s.Theater?.Seats?.Count ?? 0);
                var occupancyRate = totalCapacity > 0 ? (int)(filmData.TotalSeats * 100 / totalCapacity) : 0;

                result.Add(new TopFilm
                {
                    Id = filmData.Movie.MovieId.ToString(),
                    Title = filmData.Movie.Title,
                    Reservations = filmData.Reservations,
                    OccupancyRate = occupancyRate,
                    Revenue = filmData.Revenue,
                    ImageUrl = !string.IsNullOrEmpty(filmData.Movie.PosterUrls)
                        ? filmData.Movie.PosterUrls.Split(',')[0]
                        : "/images/films/default.jpg"
                });
            }

            return result;
        }

        public async Task<List<RecentReservation>> GetRecentReservationsAsync()
        {
            var recentReservations = await _context.Set<Reservation>()
                .Where(r => r.Status == ReservationStatus.Confirmed)
                .OrderByDescending(r => r.CreatedAt)
                .Take(20)
                .Include(r => r.AppUser)
                .Include(r => r.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Showtime)
                    .ThenInclude(s => s.Cinema)
                .Include(r => r.Seats)
                .ToListAsync();

            var result = new List<RecentReservation>();

            foreach (var reservation in recentReservations)
            {
                result.Add(new RecentReservation
                {
                    Id = reservation.ReservationId.ToString(),
                    Customer = new CustomerInfo
                    {
                        Id = reservation.AppUserId,
                        Name = $"{reservation.AppUser?.FirstName} {reservation.AppUser?.LastName}",
                        Email = reservation.AppUser?.Email ?? ""
                    },
                    Film = new FilmInfo
                    {
                        Id = reservation.Showtime?.Movie?.MovieId.ToString() ?? "",
                        Title = reservation.Showtime?.Movie?.Title ?? "",
                        Duration = reservation.Showtime?.Movie?.Duration ?? ""
                    },
                    Cinema = new CinemaInfo
                    {
                        Id = reservation.Showtime?.Cinema?.CinemaId.ToString() ?? "",
                        Name = reservation.Showtime?.Cinema?.Name ?? "",
                        City = reservation.Showtime?.Cinema?.City ?? ""
                    },
                    Showtime = reservation.Showtime?.StartTime ?? DateTime.UtcNow,
                    Tickets = reservation.NumberOfSeats,
                    TotalAmount = (decimal)reservation.TotalPrice,
                    Status = reservation.Status.ToString().ToLower(),
                    CreatedAt = reservation.CreatedAt
                });
            }

            return result;
        }

        public async Task<List<ActivityLog>> GetActivityLogsAsync()
        {
            var recentReservations = await _context.Set<Reservation>()
                .Where(r => r.Status == ReservationStatus.Confirmed)
                .OrderByDescending(r => r.CreatedAt)
                .Take(50)
                .Include(r => r.AppUser)
                .Include(r => r.Showtime)
                    .ThenInclude(s => s.Movie)
                .ToListAsync();

            var activityLogs = new List<ActivityLog>();

            foreach (var reservation in recentReservations)
            {
                var timeAgo = GetRelativeTime(reservation.CreatedAt);

                activityLogs.Add(new ActivityLog
                {
                    Id = $"res-{reservation.ReservationId}",
                    Type = "reservation",
                    Action = "create",
                    Message = $"Nouvelle réservation pour '{reservation.Showtime?.Movie?.Title}'",
                    UserId = reservation.AppUserId,
                    UserName = $"{reservation.AppUser?.FirstName} {reservation.AppUser?.LastName}",
                    Timestamp = reservation.CreatedAt,
                    RelativeTime = timeAgo,
                    Metadata = new Dictionary<string, string>
                    {
                        { "reservationId", reservation.ReservationId.ToString() },
                        { "filmId", reservation.Showtime?.MovieId.ToString() ?? "" },
                        { "showtimeId", reservation.ShowtimeId.ToString() },
                        { "tickets", reservation.NumberOfSeats.ToString() },
                        { "totalAmount", reservation.TotalPrice.ToString("F2") }
                    }
                });
            }

            return activityLogs;
        }

        private string GetRelativeTime(DateTime timestamp)
        {
            var timeSpan = DateTime.UtcNow - timestamp;

            if (timeSpan.TotalMinutes < 1)
                return "À l'instant";
            if (timeSpan.TotalMinutes < 60)
                return $"Il y a {(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")}";
            if (timeSpan.TotalHours < 24)
                return $"Il y a {(int)timeSpan.TotalHours} heure{(timeSpan.TotalHours >= 2 ? "s" : "")}";
            if (timeSpan.TotalDays < 30)
                return $"Il y a {(int)timeSpan.TotalDays} jour{(timeSpan.TotalDays >= 2 ? "s" : "")}";
            
            return $"Il y a {(int)(timeSpan.TotalDays / 30)} mois";
        }

        // Méthodes pour les statistiques d'incidents (à implémenter)
        public async Task<IncidentStats> GetIncidentStatsAsync()
        {
            // TODO: Implémenter la logique pour récupérer les statistiques d'incidents
            return new IncidentStats();
        }

        public async Task<IncidentChartData> GetIncidentChartDataAsync(string period)
        {
            // TODO: Implémenter la logique pour récupérer les données du graphique d'incidents
            return new IncidentChartData();
        }

        public async Task<List<TopIncidentType>> GetTopIncidentTypesAsync()
        {
            // TODO: Implémenter la logique pour récupérer les types d'incidents les plus fréquents
            return new List<TopIncidentType>();
        }

        public async Task<List<CinephoriaServer.API.Models.MongooDb.IncidentDto>> GetRecentIncidentsAsync(int limit = 10)
        {
            // TODO: Implémenter la logique pour récupérer les incidents récents
            return new List<CinephoriaServer.API.Models.MongooDb.IncidentDto>();
        }

        public async Task<List<IncidentActivity>> GetIncidentActivitiesAsync()
        {
            // TODO: Implémenter la logique pour récupérer les activités d'incidents
            return new List<IncidentActivity>();
        }
    }
}
