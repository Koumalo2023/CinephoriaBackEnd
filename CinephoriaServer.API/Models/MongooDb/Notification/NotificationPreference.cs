using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CinephoriaServer.API.Models.MongooDb
{
    public class NotificationPreference
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("emailEnabled")]
        public bool EmailEnabled { get; set; } = true;

        [BsonElement("appEnabled")]
        public bool AppEnabled { get; set; } = true;

        [BsonElement("preferences")]
        public Dictionary<NotificationType, bool> Preferences { get; set; } = new Dictionary<NotificationType, bool>
        {
            { NotificationType.EmailNewReservation, true },
            { NotificationType.EmailCanceledReservation, true },
            { NotificationType.EmailNewUser, true },
            { NotificationType.EmailSystemAlerts, true },
            { NotificationType.AppNewReservation, true },
            { NotificationType.AppCanceledReservation, true },
            { NotificationType.AppNewUser, true },
            { NotificationType.AppSystemAlerts, true }
        };

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}