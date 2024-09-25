using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CinephoriaBackEnd.Models
{
    public class Incident
    {
        [BsonId] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("RoomId")]
        public int RoomId { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("ReporterId")]
        public string ReporterId { get; set; } 

        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Pas de navigation automatique en MongoDB
        [BsonIgnore]
        public Room Room { get; set; }

        [BsonIgnore]
        public AppUser Reporter { get; set; }
    }

}
