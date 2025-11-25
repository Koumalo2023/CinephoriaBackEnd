using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CinephoriaServer.API.Models.MongooDb
{
    public class NotificationModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("type")]
        public NotificationType Type { get; set; }

        [BsonElement("recipientId")]
        public string RecipientId { get; set; } = string.Empty;

        [BsonElement("recipientEmail")]
        public string? RecipientEmail { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("data")]
        public Dictionary<string, object>? Data { get; set; }

        [BsonElement("isRead")]
        public bool IsRead { get; set; } = false;

        [BsonElement("sentAt")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}