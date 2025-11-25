using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.DTOs.Showtime;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.API.Services
{
    public class ShowtimeStatusService : IShowtimeStatusService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimeStatusService> _logger;

        public ShowtimeStatusService(
            IUnitOfWorkPostgres unitOfWork,
            IMapper mapper,
            ILogger<ShowtimeStatusService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> UpdateAllShowtimeStatusesAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                _logger.LogInformation("Début de la mise à jour automatique des statuts des séances à {Now}", now);

                var showtimes = await _unitOfWork.Showtimes.GetShowtimesForStatusUpdateAsync();
                int updatedCount = 0;

                foreach (var showtime in showtimes)
                {
                    var previousStatus = showtime.Status;
                    var newStatus = DetermineShowtimeStatus(showtime, now);

                    if (previousStatus != newStatus)
                    {
                        showtime.Status = newStatus;
                        showtime.UpdatedAt = DateTime.UtcNow;

                        // Mettre à jour les heures réelles si nécessaire
                        if (newStatus == EnumConfig.ShowtimeStatus.Ongoing && showtime.ActualStartTime == null)
                        {
                            showtime.ActualStartTime = DateTime.SpecifyKind(now, DateTimeKind.Utc);
                        }
                        else if (newStatus == EnumConfig.ShowtimeStatus.Completed && showtime.ActualEndTime == null)
                        {
                            showtime.ActualEndTime = DateTime.SpecifyKind(now, DateTimeKind.Utc);
                        }

                        await _unitOfWork.Showtimes.UpdateAsync(showtime);
                        updatedCount++;

                        _logger.LogInformation(
                            "Séance {ShowtimeId} mise à jour: {PreviousStatus} -> {NewStatus}",
                            showtime.ShowtimeId, previousStatus, newStatus);
                    }
                }

                if (updatedCount > 0)
                {
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Mise à jour terminée: {UpdatedCount} séances modifiées", updatedCount);
                }
                else
                {
                    _logger.LogInformation("Aucune séance à mettre à jour");
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour automatique des statuts des séances");
                throw;
            }
        }

        public async Task<List<ShowtimeStatusDto>> GetShowtimesByStatusAsync(EnumConfig.ShowtimeStatus status)
        {
            try
            {
                var showtimes = await _unitOfWork.Showtimes.GetShowtimesByStatusAsync(status);
                return _mapper.Map<List<ShowtimeStatusDto>>(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des séances avec le statut {Status}", status);
                return new List<ShowtimeStatusDto>();
            }
        }

        public async Task<List<ShowtimeStatusDto>> GetAllShowtimesWithStatusAsync()
        {
            try
            {
                var showtimes = await _unitOfWork.Showtimes.GetShowtimesWithMoviesAsync();
                return _mapper.Map<List<ShowtimeStatusDto>>(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les séances avec statuts");
                return new List<ShowtimeStatusDto>();
            }
        }

        public async Task<List<ShowtimeStatusDto>> GetUpcomingShowtimesAsync()
        {
            try
            {
                var showtimes = await _unitOfWork.Showtimes.GetUpcomingShowtimesAsync();
                return _mapper.Map<List<ShowtimeStatusDto>>(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des séances à venir");
                return new List<ShowtimeStatusDto>();
            }
        }

        public async Task<List<ShowtimeStatusDto>> GetOngoingShowtimesAsync()
        {
            try
            {
                var showtimes = await _unitOfWork.Showtimes.GetOngoingShowtimesAsync();
                return _mapper.Map<List<ShowtimeStatusDto>>(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des séances en cours");
                return new List<ShowtimeStatusDto>();
            }
        }

        public async Task<bool> UpdateShowtimeStatusAsync(int showtimeId, EnumConfig.ShowtimeStatus newStatus)
        {
            try
            {
                var showtime = await _unitOfWork.Showtimes.GetByIdAsync(showtimeId);
                if (showtime == null)
                {
                    _logger.LogWarning("Séance {ShowtimeId} non trouvée pour mise à jour de statut", showtimeId);
                    return false;
                }

                var previousStatus = showtime.Status;
                showtime.Status = newStatus;
                showtime.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Showtimes.UpdateAsync(showtime);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "Statut de la séance {ShowtimeId} mis à jour manuellement: {PreviousStatus} -> {NewStatus}",
                    showtimeId, previousStatus, newStatus);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour manuelle du statut de la séance {ShowtimeId}", showtimeId);
                return false;
            }
        }

        public async Task<ShowtimeStatusStatsDto> GetShowtimeStatsAsync()
        {
            try
            {
                var upcomingCount = await _unitOfWork.Showtimes.GetShowtimeCountByStatusAsync(EnumConfig.ShowtimeStatus.Upcoming);
                var ongoingCount = await _unitOfWork.Showtimes.GetShowtimeCountByStatusAsync(EnumConfig.ShowtimeStatus.Ongoing);
                var completedCount = await _unitOfWork.Showtimes.GetShowtimeCountByStatusAsync(EnumConfig.ShowtimeStatus.Completed);
                var cancelledCount = await _unitOfWork.Showtimes.GetShowtimeCountByStatusAsync(EnumConfig.ShowtimeStatus.Cancelled);
                var totalCount = await _unitOfWork.Showtimes.GetAllAsync();

                return new ShowtimeStatusStatsDto
                {
                    UpcomingCount = upcomingCount,
                    OngoingCount = ongoingCount,
                    CompletedCount = completedCount,
                    CancelledCount = cancelledCount,
                    TotalCount = totalCount.Count(),
                    LastUpdate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques des séances");
                return new ShowtimeStatusStatsDto();
            }
        }

        public async Task<List<object>> GetMoviesWithShowtimesAsync()
        {
            try
            {
                return await _unitOfWork.Showtimes.GetMoviesWithShowtimesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des films avec séances");
                return new List<object>();
            }
        }

        public async Task<List<object>> GetRecentMoviesWithShowtimesAsync()
        {
            try
            {
                return await _unitOfWork.Showtimes.GetRecentMoviesWithShowtimesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des films récents avec séances");
                return new List<object>();
            }
        }

        /// <summary>
        /// Met à jour les statuts des séances (alias pour UpdateAllShowtimeStatusesAsync)
        /// </summary>
        public async Task<int> UpdateShowtimeStatusesAsync()
        {
            return await UpdateAllShowtimeStatusesAsync();
        }

        private EnumConfig.ShowtimeStatus DetermineShowtimeStatus(Models.PostgresqlDb.Showtime showtime, DateTime currentTime)
        {
            if (showtime.Status == EnumConfig.ShowtimeStatus.Cancelled)
                return EnumConfig.ShowtimeStatus.Cancelled;

            if (currentTime < showtime.StartTime)
                return EnumConfig.ShowtimeStatus.Upcoming;

            if (currentTime >= showtime.StartTime && currentTime < showtime.EndTime)
                return EnumConfig.ShowtimeStatus.Ongoing;

            return EnumConfig.ShowtimeStatus.Completed;
        }
    }
}