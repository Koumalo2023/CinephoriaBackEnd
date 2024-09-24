using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PosterUrl { get; set; }

        public int AgeLimit { get; set; }

        public bool Favorite { get; set; }

        public float RatingAverage { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public ICollection<Showtime> Showtimes { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }

}
