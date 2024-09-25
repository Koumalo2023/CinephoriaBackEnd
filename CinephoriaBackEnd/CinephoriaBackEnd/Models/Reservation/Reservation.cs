using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationId { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [Required]
        public int ShowtimeId { get; set; }

        [Required]
        public int SeatNumber { get; set; }

        [Required]
        public float Price { get; set; }

        public string QrCode { get; set; }

        [Required]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        [ForeignKey("ShowtimeId")]
        public Showtime Showtime { get; set; }
    }

}
