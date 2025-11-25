using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services.Notification
{
    public interface INotificationService
    {
        Task SendNotificationAsync(NotificationType type, string recipientId, string? recipientEmail, 
                                 string title, string message, Dictionary<string, object>? data = null);
        
        Task SendEmailNotificationAsync(NotificationType type, string recipientEmail, 
                                      string subject, string message, Dictionary<string, object>? data = null);
        
        Task SendAppNotificationAsync(NotificationType type, string recipientId, 
                                    string title, string message, Dictionary<string, object>? data = null);
        
        Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(string userId, int limit = 50, int skip = 0);
        Task MarkAsReadAsync(string notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        
        // Méthodes spécifiques pour les différents types de notifications
        Task SendNewReservationNotificationAsync(string userId, ReservationDto reservationDetails);
        Task SendCanceledReservationNotificationAsync(string userId, ReservationDto reservationDetails);
        Task SendNewUserNotificationAsync(string userId, UserProfileDto userDetails);
        Task SendSystemAlertNotificationAsync(string message, string severity = "info");
    }
}