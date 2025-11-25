

namespace CinephoriaServer.API.Services
{
    public class ShowtimeStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ShowtimeStatusBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Mise à jour toutes les 5 minutes

        public ShowtimeStatusBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ShowtimeStatusBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service de mise à jour automatique des statuts des séances démarré");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var showtimeStatusService = scope.ServiceProvider.GetRequiredService<IShowtimeStatusService>();
                        await showtimeStatusService.UpdateShowtimeStatusesAsync();
                    }

                    _logger.LogDebug("Mise à jour automatique des statuts terminée avec succès");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la mise à jour automatique des statuts des séances");
                }

                // Attendre l'intervalle spécifié avant la prochaine exécution
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Service de mise à jour automatique des statuts des séances arrêté");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Démarrage du service de mise à jour automatique des statuts des séances");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Arrêt du service de mise à jour automatique des statuts des séances");
            await base.StopAsync(cancellationToken);
        }
    }
}