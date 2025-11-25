using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services
{
    public interface IReservationReminderService
    {
        /// <summary>
        /// Envoie les rappels pour les réservations en attente de paiement
        /// </summary>
        Task<int> SendPaymentRemindersAsync();

        /// <summary>
        /// Envoie les rappels pour les séances à venir
        /// </summary>
        Task<int> SendUpcomingShowtimeRemindersAsync();

        /// <summary>
        /// Envoie les notifications d'expiration imminente
        /// </summary>
        Task<int> SendExpirationWarningsAsync();

        /// <summary>
        /// Envoie les notifications de confirmation de réservation
        /// </summary>
        Task<int> SendConfirmationNotificationsAsync();

        /// <summary>
        /// Envoie les notifications d'annulation
        /// </summary>
        Task<int> SendCancellationNotificationsAsync();

        /// <summary>
        /// Envoie les notifications No Show
        /// </summary>
        Task<int> SendNoShowNotificationsAsync();

        /// <summary>
        /// Récupère les réservations nécessitant des rappels de paiement
        /// </summary>
        Task<List<Models.PostgresqlDb.Reservation>> GetReservationsNeedingPaymentRemindersAsync();

        /// <summary>
        /// Récupère les réservations nécessitant des rappels de séance
        /// </summary>
        Task<List<Models.PostgresqlDb.Reservation>> GetReservationsNeedingShowtimeRemindersAsync();

        /// <summary>
        /// Récupère les statistiques d'envoi de notifications
        /// </summary>
        Task<ReminderStatsDto> GetReminderStatsAsync();
    }

    public class ReminderStatsDto
    {
        public int PaymentRemindersSent { get; set; }
        public int ShowtimeRemindersSent { get; set; }
        public int ExpirationWarningsSent { get; set; }
        public int ConfirmationsSent { get; set; }
        public int CancellationsSent { get; set; }
        public int NoShowNotificationsSent { get; set; }
        public DateTime LastUpdate { get; set; }
        public int TotalSentToday { get; set; }
        public int FailedSends { get; set; }
    }

    public class EmailTemplate
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
    }

    public static class EmailTemplates
    {
        public static EmailTemplate PaymentReminderTemplate => new()
        {
            Subject = "Rappel de paiement - Votre réservation Cinephoria",
            Body = @"
                <h2>Rappel de paiement</h2>
                <p>Bonjour {UserName},</p>
                <p>Votre réservation pour le film <strong>{MovieTitle}</strong> est en attente de paiement.</p>
                <p><strong>Détails de la séance :</strong></p>
                <ul>
                    <li>Date : {ShowtimeDate}</li>
                    <li>Heure : {ShowtimeTime}</li>
                    <li>Salle : {TheaterName}</li>
                    <li>Cinéma : {CinemaName}</li>
                </ul>
                <p>Montant à payer : <strong>{TotalPrice}€</strong></p>
                <p>Date limite de paiement : <strong>{PaymentDueDate}</strong></p>
                <p>Pour finaliser votre réservation, veuillez procéder au paiement dans les plus brefs délais.</p>
                <p>Cordialement,<br>L'équipe Cinephoria</p>"
        };

        public static EmailTemplate ShowtimeReminderTemplate => new()
        {
            Subject = "Rappel - Votre séance Cinephoria approche",
            Body = @"
                <h2>Rappel de séance</h2>
                <p>Bonjour {UserName},</p>
                <p>Nous vous rappelons votre séance pour le film <strong>{MovieTitle}</strong> qui aura lieu demain.</p>
                <p><strong>Détails de la séance :</strong></p>
                <ul>
                    <li>Date : {ShowtimeDate}</li>
                    <li>Heure : {ShowtimeTime}</li>
                    <li>Salle : {TheaterName}</li>
                    <li>Cinéma : {CinemaName}</li>
                    <li>Adresse : {CinemaAddress}</li>
                </ul>
                <p>Nous vous recommandons d'arriver 15 minutes avant le début de la séance.</p>
                <p>Votre QR code sera disponible dans votre espace personnel.</p>
                <p>Bon film !<br>L'équipe Cinephoria</p>"
        };

        public static EmailTemplate ExpirationWarningTemplate => new()
        {
            Subject = "Attention - Votre réservation Cinephoria expire bientôt",
            Body = @"
                <h2>Expiration imminente</h2>
                <p>Bonjour {UserName},</p>
                <p>Votre réservation pour le film <strong>{MovieTitle}</strong> expire dans moins de 2 heures.</p>
                <p><strong>Détails de la séance :</strong></p>
                <ul>
                    <li>Date : {ShowtimeDate}</li>
                    <li>Heure : {ShowtimeTime}</li>
                    <li>Salle : {TheaterName}</li>
                </ul>
                <p>Date limite de paiement : <strong>{PaymentDueDate}</strong></p>
                <p>Pour éviter l'annulation automatique, veuillez procéder au paiement rapidement.</p>
                <p>Cordialement,<br>L'équipe Cinephoria</p>"
        };

        public static EmailTemplate ConfirmationTemplate => new()
        {
            Subject = "Confirmation de réservation - Cinephoria",
            Body = @"
                <h2>Confirmation de réservation</h2>
                <p>Bonjour {UserName},</p>
                <p>Votre réservation pour le film <strong>{MovieTitle}</strong> a été confirmée.</p>
                <p><strong>Détails de la séance :</strong></p>
                <ul>
                    <li>Date : {ShowtimeDate}</li>
                    <li>Heure : {ShowtimeTime}</li>
                    <li>Salle : {TheaterName}</li>
                    <li>Cinéma : {CinemaName}</li>
                    <li>Nombre de places : {NumberOfSeats}</li>
                    <li>Montant payé : {TotalPrice}€</li>
                </ul>
                <p>Votre QR code est disponible dans votre espace personnel.</p>
                <p>Nous vous souhaitons un excellent film !</p>
                <p>Cordialement,<br>L'équipe Cinephoria</p>"
        };

        public static EmailTemplate CancellationTemplate => new()
        {
            Subject = "Annulation de réservation - Cinephoria",
            Body = @"
                <h2>Annulation de réservation</h2>
                <p>Bonjour {UserName},</p>
                <p>Votre réservation pour le film <strong>{MovieTitle}</strong> a été annulée.</p>
                <p><strong>Raison :</strong> {CancellationReason}</p>
                <p><strong>Détails de la séance annulée :</strong></p>
                <ul>
                    <li>Date : {ShowtimeDate}</li>
                    <li>Heure : {ShowtimeTime}</li>
                    <li>Salle : {TheaterName}</li>
                </ul>
                <p>Si vous avez des questions, n'hésitez pas à nous contacter.</p>
                <p>Cordialement,<br>L'équipe Cinephoria</p>"
        };

        public static EmailTemplate NoShowTemplate => new()
        {
            Subject = "Absence à la séance - Cinephoria",
            Body = @"
                <h2>Absence à la séance</h2>
                <p>Bonjour {UserName},</p>
                <p>Nous constatons que vous n'êtes pas venu à votre séance pour le film <strong>{MovieTitle}</strong>.</p>
                <p><strong>Détails de la séance :</strong></p>
                <ul>
                    <li>Date : {ShowtimeDate}</li>
                    <li>Heure : {ShowtimeTime}</li>
                    <li>Salle : {TheaterName}</li>
                </ul>
                <p>Votre réservation a été marquée comme 'No Show'.</p>
                <p>Si vous avez rencontré un problème, veuillez nous contacter.</p>
                <p>Cordialement,<br>L'équipe Cinephoria</p>"
        };
    }
}