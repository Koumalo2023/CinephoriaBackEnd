using CinephoriaServer.API.Configurations;

namespace CinephoriaServer.API.DTOs.Showtime
{
    public class ShowtimeStatusDto
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public EnumConfig.ShowtimeStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public Guid MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public Guid TheaterId { get; set; }
        public string TheaterName { get; set; } = string.Empty;
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public bool IsFull => AvailableSeats <= 0;
    }

    public class ShowtimeStatusUpdateDto
    {
        public EnumConfig.ShowtimeStatus Status { get; set; }
    }

    public class ShowtimeStatusStatsDto
    {
        public int UpcomingCount { get; set; }
        public int OngoingCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int TotalCount { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}