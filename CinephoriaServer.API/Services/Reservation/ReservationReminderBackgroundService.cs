
namespace CinephoriaServer.API.Services
{
    public class ReservationReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<ReservationReminderBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30); // Exécution toutes les 30 minutes

        public ReservationReminderBackgroundService(
            ILogger<ReservationReminderBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service de rappels de réservation démarré. Intervalle : {Interval} minutes", _interval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var reminderService = scope.ServiceProvider.GetRequiredService<IReservationReminderService>();

                    await ExecuteReminderTasksAsync(reminderService);
                    
                    _logger.LogInformation("Cycle de rappels de réservation terminé avec succès");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de l'exécution du cycle de rappels de réservation");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Service de rappels de réservation arrêté");
        }

        private async Task ExecuteReminderTasksAsync(IReservationReminderService reminderService)
        {
            var stats = new List<string>();

            try
            {
                // 1. Rappels de paiement (toutes les 24 heures)
                var paymentReminders = await reminderService.SendPaymentRemindersAsync();
                if (paymentReminders > 0)
                {
                    stats.Add($"{paymentReminders} rappels de paiement");
                }

                // 2. Avertissements d'expiration (toutes les 30 minutes)
                var expirationWarnings = await reminderService.SendExpirationWarningsAsync();
                if (expirationWarnings > 0)
                {
                    stats.Add($"{expirationWarnings} avertissements d'expiration");
                }

                // 3. Notifications de confirmation (immédiat après confirmation)
                var confirmations = await reminderService.SendConfirmationNotificationsAsync();
                if (confirmations > 0)
                {
                    stats.Add($"{confirmations} notifications de confirmation");
                }

                // 4. Notifications d'annulation (immédiat après annulation)
                var cancellations = await reminderService.SendCancellationNotificationsAsync();
                if (cancellations > 0)
                {
                    stats.Add($"{cancellations} notifications d'annulation");
                }

                // 5. Notifications No Show (immédiat après marquage No Show)
                var noShows = await reminderService.SendNoShowNotificationsAsync();
                if (noShows > 0)
                {
                    stats.Add($"{noShows} notifications No Show");
                }

                // 6. Rappels de séance (une fois par jour, le matin)
                if (DateTime.UtcNow.Hour == 8) // 8h UTC = 9h heure française
                {
                    var showtimeReminders = await reminderService.SendUpcomingShowtimeRemindersAsync();
                    if (showtimeReminders > 0)
                    {
                        stats.Add($"{showtimeReminders} rappels de séance");
                    }
                }

                // Log des statistiques si des notifications ont été envoyées
                if (stats.Any())
                {
                    _logger.LogInformation("Rappels envoyés : {Stats}", string.Join(", ", stats));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'exécution des tâches de rappel");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Arrêt du service de rappels de réservation en cours...");
            await base.StopAsync(cancellationToken);
        }
    }
}