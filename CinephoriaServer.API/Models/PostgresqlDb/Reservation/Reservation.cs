using static CinephoriaServer.API.Configurations.EnumConfig;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CinephoriaServer.API.Configurations.Extensions;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class Reservation : BaseEntity
    {
        private int _numberOfSeats;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationId { get; set; }

        [Required]
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }

        [Required]
        [ForeignKey("Showtime")]
        public int ShowtimeId { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Le prix total doit être positif.")]
        public float TotalPrice { get; set; }

        [Required]
        public string QrCode { get; set; } = string.Empty;

        public bool IsValidated { get; set; } = false;

        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        // Nouveaux champs pour la gestion avancée
        public DateTime? PaymentDueDate { get; set; } // Date limite de paiement
        public DateTime? PaymentDate { get; set; }    // Date effective de paiement
        public DateTime? CheckInTime { get; set; }    // Heure d'arrivée au cinéma
        public DateTime? CheckOutTime { get; set; }   // Heure de départ du cinéma
        public bool IsNoShow { get; set; } = false;   // Indicateur de non-présentation
        public string CancellationReason { get; set; } = string.Empty; // Raison d'annulation
        public DateTime? ExpirationDate { get; set; } // Date d'expiration automatique

        // Propriétés pour le système de notifications et rappels
        public bool ConfirmationSent { get; set; } = false; // Notification de confirmation envoyée
        public bool CancellationSent { get; set; } = false; // Notification d'annulation envoyée
        public bool NoShowSent { get; set; } = false; // Notification No Show envoyée
        public DateTime? LastReminderSent { get; set; } // Dernier rappel envoyé

        public int NumberOfSeats
        {
            get => _numberOfSeats;
            set => _numberOfSeats = value;
        }

        public AppUser AppUser { get; set; }
        public Showtime Showtime { get; set; }
        public Movie Movie => Showtime?.Movie;

        // Méthodes utilitaires
        public bool IsExpired()
        {
            return Status == ReservationStatus.Pending &&
                   PaymentDueDate.HasValue &&
                   PaymentDueDate.Value < DateTime.UtcNow;
        }

        public bool CanBeCancelled()
        {
            return Status == ReservationStatus.Pending ||
                   Status == ReservationStatus.Confirmed;
        }

        public bool CanCheckIn()
        {
            return Status == ReservationStatus.Confirmed &&
                   Showtime?.StartTime <= DateTime.UtcNow.AddMinutes(30) &&
                   Showtime?.EndTime > DateTime.UtcNow;
        }

        public bool IsActive()
        {
            return Status == ReservationStatus.Pending ||
                   Status == ReservationStatus.Confirmed;
        }
    }
}
