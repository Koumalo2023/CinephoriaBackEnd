using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CinemaId { get; set; }

        public int NumberOfSeats { get; set; }

        public string ProjectionQuality { get; set; }

        // Navigation properties
        public Cinema Cinema { get; set; }

        public ICollection<Showtime> Showtimes { get; set; }

        public ICollection<Incident> Incidents { get; set; }
    }

}
