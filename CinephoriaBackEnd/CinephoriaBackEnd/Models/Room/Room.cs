using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }

        [Required]
        public int CinemaId { get; set; }

        [Required]
        public int NumberOfSeats { get; set; }

        [Required]
        public string ProjectionQuality { get; set; }

        // Navigation properties
        [ForeignKey("CinemaId")]
        public Cinema Cinema { get; set; }

        public ICollection<Showtime> Showtimes { get; set; }
        public ICollection<Incident> Incidents { get; set; }
    }

}
