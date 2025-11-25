using MailKit.Security;
using MailKit.Net.Smtp; // Utilisez SmtpClient de MailKit
using MimeKit;

namespace CinephoriaServer.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            // Créer le message MIME
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Patrice Simo", smtpSettings["Username"]));
            message.To.Add(new MailboxAddress("Destinataire", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = isHtml ? body : null,
                TextBody = isHtml ? null : body
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Utiliser SmtpClient de MailKit
            using (var client = new SmtpClient())
            {
                try
                {
                    // Convertir SecureSocketOptions
                    var secureSocketOptions = smtpSettings["SecureSocketOptions"] switch
                    {
                        "StartTls" => SecureSocketOptions.StartTls,
                        "SslOnConnect" => SecureSocketOptions.SslOnConnect,
                        _ => SecureSocketOptions.Auto
                    };

                    // Connexion au serveur SMTP
                    await client.ConnectAsync(smtpSettings["Server"], int.Parse(smtpSettings["Port"]), secureSocketOptions);

                    // Authentification
                    await client.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);

                    // Envoi de l'email
                    await client.SendAsync(message);

                    // Déconnexion
                    await client.DisconnectAsync(true);
                    
                    _logger.LogInformation("Email envoyé avec succès à {ToEmail}", toEmail);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de l'envoi de l'email à {ToEmail}", toEmail);
                    return false;
                }
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

            _logger.LogInformation("Envoi en masse : {Successful}/{Total} emails envoyés",
                successfulSends, recipients.Count);

            return successfulSends;
        }

        public Task<bool> ValidateConfigurationAsync()
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var server = smtpSettings["Server"];
                var port = smtpSettings["Port"];
                var username = smtpSettings["Username"];
                var password = smtpSettings["Password"];

                var isValid = !string.IsNullOrEmpty(server) &&
                             !string.IsNullOrEmpty(port) &&
                             !string.IsNullOrEmpty(username) &&
                             !string.IsNullOrEmpty(password);

                return Task.FromResult(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la validation de la configuration SMTP");
                return Task.FromResult(false);
            }
        }

        public Task<EmailStatsDto> GetEmailStatsAsync()
        {
            // Pour l'instant, retourner des statistiques basiques
            return Task.FromResult(new EmailStatsDto
            {
                IsConfigured = true,
                Provider = "SMTP",
                LastSent = DateTime.UtcNow
            });
        }
    }
}