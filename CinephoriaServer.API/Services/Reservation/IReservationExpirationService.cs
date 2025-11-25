using CinephoriaServer.API.Configurations;

namespace CinephoriaServer.API.Services
{
    public interface IReservationExpirationService
    {
        /// <summary>
        /// Met à jour automatiquement les statuts des réservations expirées
        /// </summary>
        Task<int> UpdateExpiredReservationsAsync();

        /// <summary>
        /// Marque les réservations comme "No Show" après la fin de la séance
        /// </summary>
        Task<int> UpdateNoShowReservationsAsync();

        /// <summary>
        /// Met à jour les réservations comme "Completed" après la séance
        /// </summary>
        Task<int> UpdateCompletedReservationsAsync();

        /// <summary>
        /// Récupère les réservations expirées
        /// </summary>
        Task<List<Models.PostgresqlDb.Reservation>> GetExpiredReservationsAsync();

        /// <summary>
        /// Récupère les réservations "No Show"
        /// </summary>
        Task<List<Models.PostgresqlDb.Reservation>> GetNoShowReservationsAsync();

        /// <summary>
        /// Récupère les statistiques d'expiration
        /// </summary>
        Task<ReservationExpirationStatsDto> GetExpirationStatsAsync();

        /// <summary>
        /// Force l'expiration d'une réservation spécifique
        /// </summary>
        Task<bool> ForceExpireReservationAsync(int reservationId);

        /// <summary>
        /// Récupère les réservations en attente de paiement
        /// </summary>
        Task<List<Models.PostgresqlDb.Reservation>> GetPendingPaymentReservationsAsync();
    }

    public class ReservationExpirationStatsDto
    {
        public int PendingCount { get; set; }
        public int ExpiredCount { get; set; }
        public int NoShowCount { get; set; }
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }
        public DateTime LastUpdate { get; set; }
        public int ExpiredThisWeek { get; set; }
        public int NoShowThisWeek { get; set; }
    }
}