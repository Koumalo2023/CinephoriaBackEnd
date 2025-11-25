namespace CinephoriaServer.API.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Envoie un email
        /// </summary>
        /// <param name="to">Adresse email du destinataire</param>
        /// <param name="subject">Sujet de l'email</param>
        /// <param name="body">Corps de l'email</param>
        /// <param name="isHtml">Indique si le corps est en HTML</param>
        /// <returns>True si l'envoi a réussi, false sinon</returns>
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Envoie un email à plusieurs destinataires
        /// </summary>
        /// <param name="recipients">Liste des adresses email des destinataires</param>
        /// <param name="subject">Sujet de l'email</param>
        /// <param name="body">Corps de l'email</param>
        /// <param name="isHtml">Indique si le corps est en HTML</param>
        /// <returns>Nombre d'emails envoyés avec succès</returns>
        Task<int> SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Vérifie la configuration du service email
        /// </summary>
        /// <returns>True si la configuration est valide, false sinon</returns>
        Task<bool> ValidateConfigurationAsync();

        /// <summary>
        /// Récupère les statistiques d'envoi
        /// </summary>
        /// <returns>Statistiques d'envoi</returns>
        Task<EmailStatsDto> GetEmailStatsAsync();
    }

    public class EmailStatsDto
    {
        public int TotalSent { get; set; }
        public int TotalFailed { get; set; }
        public int SentToday { get; set; }
        public int FailedToday { get; set; }
        public DateTime LastSent { get; set; }
        public bool IsConfigured { get; set; }
        public string Provider { get; set; } = string.Empty;
    }
}