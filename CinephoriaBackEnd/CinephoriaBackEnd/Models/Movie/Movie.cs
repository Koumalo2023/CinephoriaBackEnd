using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string PosterUrl { get; set; }

        [Required]
        public int AgeLimit { get; set; }

        public bool Favorite { get; set; }
        public float RatingAverage { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Showtime> Showtimes { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }

}
