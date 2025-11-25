using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CinephoriaServer.API.Models.MongooDb
{
    public class AdminDashboard
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("data")]
        public BsonDocument Data { get; set; } = new BsonDocument();

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("period")]
        public string Period { get; set; } = string.Empty; // "day", "week", "month", "year"

        // Pour les statistiques
        [BsonElement("reservationsToday")]
        public int ReservationsToday { get; set; }

        [BsonElement("reservationsTrend")]
        public int ReservationsTrend { get; set; }

        [BsonElement("revenueToday")]
        public decimal RevenueToday { get; set; }

        [BsonElement("revenueTrend")]
        public int RevenueTrend { get; set; }

        [BsonElement("visitorsToday")]
        public int VisitorsToday { get; set; }

        [BsonElement("visitorsTrend")]
        public int VisitorsTrend { get; set; }

        [BsonElement("occupancyRate")]
        public int OccupancyRate { get; set; }

        [BsonElement("occupancyTrend")]
        public int OccupancyTrend { get; set; }
    }

    public class DashboardStats
    {
        public int ReservationsToday { get; set; }
        public int ReservationsTrend { get; set; }
        public decimal RevenueToday { get; set; }
        public int RevenueTrend { get; set; }
        public int VisitorsToday { get; set; }
        public int VisitorsTrend { get; set; }
        public int OccupancyRate { get; set; }
        public int OccupancyTrend { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ReservationChartData
    {
        public string Period { get; set; } = string.Empty;
        public List<ReservationChartItem> Data { get; set; } = new List<ReservationChartItem>();
    }

    public class ReservationChartItem
    {
        public DateTime Date { get; set; }
        public int Reservations { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopFilm
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Reservations { get; set; }
        public int OccupancyRate { get; set; }
        public decimal Revenue { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class RecentReservation
    {
        public string Id { get; set; } = string.Empty;
        public CustomerInfo Customer { get; set; } = new CustomerInfo();
        public FilmInfo Film { get; set; } = new FilmInfo();
        public CinemaInfo Cinema { get; set; } = new CinemaInfo();
        public DateTime Showtime { get; set; }
        public int Tickets { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class FilmInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
    }

    public class CinemaInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class ActivityLog
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;    
        public string Action { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string RelativeTime { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }

    // Modèles pour les statistiques d'incidents
    public class IncidentStats
    {
        public int OpenIncidents { get; set; }
        public int OpenTrend { get; set; }               // % variation vs hier
        public int ResolvedToday { get; set; }
        public int ResolvedTrend { get; set; }           // % variation vs hier
        public double AvgResolutionTime { get; set; }    // en heures
        public int ResolutionTrend { get; set; }         // % variation du temps moyen vs hier
        public int CriticalIncidents { get; set; }       // nombre d'incidents "critiques" (détection par mot-clé)
        public int CriticalTrend { get; set; }           // % variation vs hier
        public int CustomerImpactScore { get; set; }     // score estimé (0-100) : (openIncidents / totalTheaters) * 100
        public int ImpactTrend { get; set; }             // % variation vs hier
    }

    public class IncidentChartData
    {
        public List<IncidentChartItem> Data { get; set; } = new List<IncidentChartItem>();
    }

    public class IncidentChartItem
    {
        public string Date { get; set; } = string.Empty;        // ISO format : "2025-04-01"
        public int Count { get; set; }                         // nombre d'incidents ce jour
        public int Resolved { get; set; }                      // nombre résolus ce jour
    }

    public class TopIncidentType
    {
        public string Category { get; set; } = string.Empty;    // Catégorie déduite (ex: "Projecteur", "Paiement")
        public int Count { get; set; }                         // Nombre total d'incidents dans cette catégorie
        public double ResolutionRate { get; set; }             // % d'incidents résolus dans cette catégorie
        public double AvgResponseTime { get; set; }            // Temps moyen avant première réponse (en minutes)
    }

    public class IncidentActivity
    {
        public string Type { get; set; } = string.Empty;       // "created" | "statusUpdated" | "resolved"
        public string Message { get; set; } = string.Empty;    // Ex: "Incident #123 marqué comme 'Résolu' par Marie"
        public int IncidentId { get; set; }
        public string User { get; set; } = string.Empty;       // Nom de l'utilisateur ayant effectué l'action
        public string Time { get; set; } = string.Empty;       // Format "il y a 2h" ou "2025-04-01T14:23:00Z"
    }

    public class IncidentDto
    {
        public int IncidentId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;     // "OPEN" | "IN_PROGRESS" | "RESOLVED" | "CLOSED"
        public string ReportedAt { get; set; } = string.Empty; // ISO date
        public string? ResolvedAt { get; set; }                // ISO date ou null
        public int TheaterId { get; set; }
        public string TheaterName { get; set; } = string.Empty;
        public string? ReportedBy { get; set; }                // Nom/email de l'utilisateur
        public string? ResolvedBy { get; set; }                // Nom/email de l'utilisateur
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}