using Microsoft.Extensions.Logging;

namespace CinephoriaServer.API.Services
{
    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;
        private readonly EmailStatsDto _stats;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
            _stats = new EmailStatsDto
            {
                Provider = "Mock",
                IsConfigured = true,
                LastSent = DateTime.UtcNow
            };
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                // Simulation d'un délai d'envoi
                await Task.Delay(100);

                // Vérification basique de l'adresse email
                if (string.IsNullOrWhiteSpace(to) || !to.Contains('@'))
                {
                    _logger.LogWarning("Tentative d'envoi d'email à une adresse invalide : {To}", to);
                    _stats.TotalFailed++;
                    _stats.FailedToday++;
                    return false;
                }

                // Log de l'email simulé
                _logger.LogInformation("📧 EMAIL SIMULÉ - À: {To}, Sujet: {Subject}, HTML: {IsHtml}", 
                    to, subject, isHtml);

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Corps de l'email : {Body}", body);
                }

                // Mise à jour des statistiques
                _stats.TotalSent++;
                _stats.SentToday++;
                _stats.LastSent = DateTime.UtcNow;

                // Simulation d'un taux d'échec de 5% pour les tests
                var random = new Random();
                if (random.Next(100) < 5) // 5% de chance d'échec
                {
                    _logger.LogWarning("Échec simulé de l'envoi d'email à {To}", to);
                    _stats.TotalFailed++;
                    _stats.FailedToday++;
                    return false;
                }

                _logger.LogInformation("✅ Email simulé envoyé avec succès à {To}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi simulé d'email à {To}", to);
                _stats.TotalFailed++;
                _stats.FailedToday++;
                return false;
            }
        }

        public async Task<int> SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true)
        {
            var successfulSends = 0;

            foreach (var recipient in recipients)
            {
                var success = await SendEmailAsync(recipient, subject, body, isHtml);
                if (success)
                {
                    successfulSends++;
                }
            }

            _logger.LogInformation("Envoi en masse simulé : {Successful}/{Total} emails envoyés", 
                successfulSends, recipients.Count);

            return successfulSends;
        }

        public Task<bool> ValidateConfigurationAsync()
        {
            // Le service mock est toujours configuré
            return Task.FromResult(true);
        }

        public Task<EmailStatsDto> GetEmailStatsAsync()
        {
            // Réinitialiser les compteurs quotidiens si c'est un nouveau jour
            if (_stats.LastSent.Date < DateTime.UtcNow.Date)
            {
                _stats.SentToday = 0;
                _stats.FailedToday = 0;
            }

            return Task.FromResult(_stats);
        }
    }
}