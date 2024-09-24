using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Showtime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MovieId { get; set; }

        public int RoomId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Quality { get; set; }

        public float Price { get; set; }

        public int AvailableSeats { get; set; }

        // Navigation properties
        public Movie Movie { get; set; }

        public Room Room { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }

}
