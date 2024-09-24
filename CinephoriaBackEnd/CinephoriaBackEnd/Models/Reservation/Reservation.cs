using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AppUserId { get; set; }

        public int ShowtimeId { get; set; }

        public int SeatNumber { get; set; }

        public float Price { get; set; }

        public string QrCode { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public AppUser AppUser { get; set; }

        public Showtime Showtime { get; set; }
    }

}
