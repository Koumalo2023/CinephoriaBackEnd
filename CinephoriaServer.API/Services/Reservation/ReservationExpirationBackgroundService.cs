
namespace CinephoriaServer.API.Services
{
    public class ReservationExpirationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationExpirationBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Exécution toutes les 5 minutes

        public ReservationExpirationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ReservationExpirationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service d'expiration des réservations démarré");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var expirationService = scope.ServiceProvider.GetRequiredService<IReservationExpirationService>();

                    // Mettre à jour les réservations expirées
                    var expiredCount = await expirationService.UpdateExpiredReservationsAsync();
                    
                    // Mettre à jour les réservations No Show
                    var noShowCount = await expirationService.UpdateNoShowReservationsAsync();
                    
                    // Mettre à jour les réservations complétées
                    var completedCount = await expirationService.UpdateCompletedReservationsAsync();

                    if (expiredCount > 0 || noShowCount > 0 || completedCount > 0)
                    {
                        _logger.LogInformation(
                            "Mise à jour automatique des réservations: {ExpiredCount} expirées, {NoShowCount} No Show, {CompletedCount} complétées",
                            expiredCount, noShowCount, completedCount);
                    }

                    // Attendre l'intervalle suivant
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Service arrêté normalement
                    _logger.LogInformation("Service d'expiration des réservations arrêté");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de l'exécution du service d'expiration des réservations");
                    
                    // Attendre avant de réessayer en cas d'erreur
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Arrêt du service d'expiration des réservations");
            await base.StopAsync(cancellationToken);
        }
    }
}