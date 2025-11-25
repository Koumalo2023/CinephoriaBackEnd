using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.MongooDb;
using CinephoriaServer.API.Repository.EntityFramwork;
using MongoDB.Driver;

namespace CinephoriaServer.API.Repository
{
    public interface INotificationRepository : IMongoRepository<NotificationModel>
    {
        Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(string userId, int limit = 50, int skip = 0);
        Task MarkAsReadAsync(string notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<NotificationPreference?> GetUserPreferencesAsync(string userId);
        Task UpdateUserPreferencesAsync(NotificationPreference preferences);
    }

    public class NotificationRepository : MongoRepository<NotificationModel>, INotificationRepository
    {
        private readonly IMongoCollection<NotificationModel> _notificationCollection;
        private readonly IMongoCollection<NotificationPreference> _preferencesCollection;

        public NotificationRepository(MongoDbContext context) : base(context, "Notifications")
        {
            _notificationCollection = context.GetCollection<NotificationModel>("Notifications");
            _preferencesCollection = context.GetCollection<NotificationPreference>("NotificationPreferences");
        }

        public async Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(string userId, int limit = 50, int skip = 0)
        {
            return await _notificationCollection
                .Find(n => n.RecipientId == userId)
                .SortByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(string notificationId)
        {
            var filter = Builders<NotificationModel>.Filter.Eq(n => n.Id, notificationId);
            var update = Builders<NotificationModel>.Update.Set(n => n.IsRead, true);
            await _notificationCollection.UpdateOneAsync(filter, update);
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var filter = Builders<NotificationModel>.Filter.Eq(n => n.RecipientId, userId) &
                         Builders<NotificationModel>.Filter.Eq(n => n.IsRead, false);
            var update = Builders<NotificationModel>.Update.Set(n => n.IsRead, true);
            await _notificationCollection.UpdateManyAsync(filter, update);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            var filter = Builders<NotificationModel>.Filter.Eq(n => n.RecipientId, userId) &
                         Builders<NotificationModel>.Filter.Eq(n => n.IsRead, false);
            return (int)await _notificationCollection.CountDocumentsAsync(filter);
        }

        public async Task<NotificationPreference?> GetUserPreferencesAsync(string userId)
        {
            return await _preferencesCollection
                .Find(p => p.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateUserPreferencesAsync(NotificationPreference preferences)
        {
            var filter = Builders<NotificationPreference>.Filter.Eq(p => p.UserId, preferences.UserId);
            var options = new ReplaceOptions { IsUpsert = true };
            await _preferencesCollection.ReplaceOneAsync(filter, preferences, options);
        }
    }
}