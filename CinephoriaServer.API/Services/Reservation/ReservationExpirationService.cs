using CinephoriaServer.API.Configurations; 
using CinephoriaServer.API.Repository; 

namespace CinephoriaServer.API.Services
{
    public class ReservationExpirationService : IReservationExpirationService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly ILogger<ReservationExpirationService> _logger;

        public ReservationExpirationService(
            IUnitOfWorkPostgres unitOfWork,
            ILogger<ReservationExpirationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> UpdateExpiredReservationsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                _logger.LogInformation("Début de la mise à jour des réservations expirées à {Now}", now);

                var expiredReservations = await GetExpiredReservationsAsync();
                int updatedCount = 0;

                foreach (var reservation in expiredReservations)
                {
                    var previousStatus = reservation.Status;
                    reservation.Status = EnumConfig.ReservationStatus.Expired;
                    reservation.ExpirationDate = now;
                    reservation.CancellationReason = "Expiration automatique - Paiement non effectué dans le délai";

                    await _unitOfWork.Reservations.UpdateAsync(reservation);
                    updatedCount++;

                    _logger.LogInformation(
                        "Réservation {ReservationId} expirée: {PreviousStatus} -> {NewStatus}",
                        reservation.ReservationId, previousStatus, reservation.Status);
                }

                if (updatedCount > 0)
                {
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Mise à jour terminée: {UpdatedCount} réservations expirées", updatedCount);
                }
                else
                {
                    _logger.LogInformation("Aucune réservation à expirer");
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour des réservations expirées");
                throw;
            }
        }

        public async Task<int> UpdateNoShowReservationsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                _logger.LogInformation("Début de la mise à jour des réservations No Show à {Now}", now);

                var noShowReservations = await GetNoShowReservationsAsync();
                int updatedCount = 0;

                foreach (var reservation in noShowReservations)
                {
                    var previousStatus = reservation.Status;
                    reservation.Status = EnumConfig.ReservationStatus.NoShow;
                    reservation.IsNoShow = true;
                    reservation.CancellationReason = "No Show - Non présentation à la séance";

                    await _unitOfWork.Reservations.UpdateAsync(reservation);
                    updatedCount++;

                    _logger.LogInformation(
                        "Réservation {ReservationId} marquée No Show: {PreviousStatus} -> {NewStatus}",
                        reservation.ReservationId, previousStatus, reservation.Status);
                }

                if (updatedCount > 0)
                {
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Mise à jour terminée: {UpdatedCount} réservations No Show", updatedCount);
                }
                else
                {
                    _logger.LogInformation("Aucune réservation No Show à mettre à jour");
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour des réservations No Show");
                throw;
            }
        }

        public async Task<int> UpdateCompletedReservationsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                _logger.LogInformation("Début de la mise à jour des réservations complétées à {Now}", now);

                // Récupérer les réservations confirmées avec check-in
                var confirmedReservations = await _unitOfWork.Reservations
                    .FindAsync(r => r.Status == EnumConfig.ReservationStatus.Confirmed &&
                                   r.CheckInTime.HasValue);

                int updatedCount = 0;

                foreach (var reservation in confirmedReservations)
                {
                    // Charger le showtime pour vérifier l'heure de fin
                    if (reservation.ShowtimeId != 0)
                    {
                        reservation.Showtime = await _unitOfWork.Showtimes.GetByIdAsync(reservation.ShowtimeId);
                        
                        if (reservation.Showtime?.EndTime <= now)
                        {
                            var previousStatus = reservation.Status;
                            reservation.Status = EnumConfig.ReservationStatus.Completed;
                            reservation.CheckOutTime ??= now;

                            await _unitOfWork.Reservations.UpdateAsync(reservation);
                            updatedCount++;

                            _logger.LogInformation(
                                "Réservation {ReservationId} complétée: {PreviousStatus} -> {NewStatus}",
                                reservation.ReservationId, previousStatus, reservation.Status);
                        }
                    }
                }

                if (updatedCount > 0)
                {
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Mise à jour terminée: {UpdatedCount} réservations complétées", updatedCount);
                }
                else
                {
                    _logger.LogInformation("Aucune réservation à marquer comme complétée");
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour des réservations complétées");
                throw;
            }
        }

        public async Task<List<Models.PostgresqlDb.Reservation>> GetExpiredReservationsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var reservations = await _unitOfWork.Reservations
                    .FindAsync(r => r.Status == EnumConfig.ReservationStatus.Pending &&
                                   r.PaymentDueDate.HasValue &&
                                   r.PaymentDueDate.Value < now);
                
                // Charger les relations manuellement si nécessaire
                foreach (var reservation in reservations)
                {
                    if (reservation.ShowtimeId != 0)
                        reservation.Showtime = await _unitOfWork.Showtimes.GetByIdAsync(reservation.ShowtimeId);
                    if (!string.IsNullOrEmpty(reservation.AppUserId))
                        reservation.AppUser = await _unitOfWork.Users.GetByIdAsync(reservation.AppUserId);
                }
                
                return reservations.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des réservations expirées");
                return new List<Models.PostgresqlDb.Reservation>();
            }
        }

        public async Task<List<Models.PostgresqlDb.Reservation>> GetNoShowReservationsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var reservations = await _unitOfWork.Reservations
                    .FindAsync(r => r.Status == EnumConfig.ReservationStatus.Confirmed &&
                                   !r.CheckInTime.HasValue &&
                                   !r.IsNoShow);
                
                // Filtrer par showtime et charger les relations
                var result = new List<Models.PostgresqlDb.Reservation>();
                foreach (var reservation in reservations)
                {
                    if (reservation.ShowtimeId != 0)
                    {
                        reservation.Showtime = await _unitOfWork.Showtimes.GetByIdAsync(reservation.ShowtimeId);
                        if (reservation.Showtime?.EndTime <= now)
                        {
                            if (!string.IsNullOrEmpty(reservation.AppUserId))
                                reservation.AppUser = await _unitOfWork.Users.GetByIdAsync(reservation.AppUserId);
                            result.Add(reservation);
                        }
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des réservations No Show");
                return new List<Models.PostgresqlDb.Reservation>();
            }
        }

        public async Task<ReservationExpirationStatsDto> GetExpirationStatsAsync()
        {
            try
            {
                var allReservations = await _unitOfWork.Reservations.GetAllAsync();
                var pendingCount = allReservations.Count(r => r.Status == EnumConfig.ReservationStatus.Pending);
                var expiredCount = allReservations.Count(r => r.Status == EnumConfig.ReservationStatus.Expired);
                var noShowCount = allReservations.Count(r => r.Status == EnumConfig.ReservationStatus.NoShow);
                var completedCount = allReservations.Count(r => r.Status == EnumConfig.ReservationStatus.Completed);

                // Calculer les statistiques de la semaine
                var startOfWeek = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
                var expiredThisWeek = allReservations.Count(r => 
                    r.Status == EnumConfig.ReservationStatus.Expired && 
                    r.ExpirationDate >= startOfWeek);
                var noShowThisWeek = allReservations.Count(r => 
                    r.Status == EnumConfig.ReservationStatus.NoShow && 
                    r.Showtime.EndTime >= startOfWeek);

                return new ReservationExpirationStatsDto
                {
                    PendingCount = pendingCount,
                    ExpiredCount = expiredCount,
                    NoShowCount = noShowCount,
                    CompletedCount = completedCount,
                    TotalCount = allReservations.Count(),
                    LastUpdate = DateTime.UtcNow,
                    ExpiredThisWeek = expiredThisWeek,
                    NoShowThisWeek = noShowThisWeek
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques d'expiration");
                return new ReservationExpirationStatsDto();
            }
        }

        public async Task<bool> ForceExpireReservationAsync(int reservationId)
        {
            try
            {
                var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
                if (reservation == null)
                {
                    _logger.LogWarning("Réservation {ReservationId} non trouvée pour expiration forcée", reservationId);
                    return false;
                }

                if (reservation.Status != EnumConfig.ReservationStatus.Pending)
                {
                    _logger.LogWarning("Réservation {ReservationId} ne peut pas être expirée (statut: {Status})", 
                        reservationId, reservation.Status);
                    return false;
                }

                var previousStatus = reservation.Status;
                reservation.Status = EnumConfig.ReservationStatus.Expired;
                reservation.ExpirationDate = DateTime.UtcNow;
                reservation.CancellationReason = "Expiration forcée par l'administrateur";

                await _unitOfWork.Reservations.UpdateAsync(reservation);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "Réservation {ReservationId} expirée manuellement: {PreviousStatus} -> {NewStatus}",
                    reservationId, previousStatus, reservation.Status);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'expiration forcée de la réservation {ReservationId}", reservationId);
                return false;
            }
        }

        public async Task<List<Models.PostgresqlDb.Reservation>> GetPendingPaymentReservationsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var reservations = await _unitOfWork.Reservations
                    .FindAsync(r => r.Status == EnumConfig.ReservationStatus.Pending &&
                                   (!r.PaymentDueDate.HasValue || r.PaymentDueDate.Value > now));
                
                // Charger les relations manuellement si nécessaire
                foreach (var reservation in reservations)
                {
                    if (reservation.ShowtimeId != 0)
                        reservation.Showtime = await _unitOfWork.Showtimes.GetByIdAsync(reservation.ShowtimeId);
                    if (!string.IsNullOrEmpty(reservation.AppUserId))
                        reservation.AppUser = await _unitOfWork.Users.GetByIdAsync(reservation.AppUserId);
                }
                
                return reservations.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des réservations en attente de paiement");
                return new List<Models.PostgresqlDb.Reservation>();
            }
        }
    }
}