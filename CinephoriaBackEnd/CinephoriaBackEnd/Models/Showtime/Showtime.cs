using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Showtime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShowtimeId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string Quality { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public int AvailableSeats { get; set; }
  
        public string AppUserId { get; set; } 

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        [ForeignKey("AppUserId")] 
        public AppUser AppUser { get; set; } 
        public ICollection<Reservation> Reservations { get; set; }
    }

}
