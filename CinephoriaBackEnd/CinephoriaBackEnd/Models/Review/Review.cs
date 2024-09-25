using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        [Required]
        public int MovieId { get; set; } // Foreign Key

        [Required]
        public string AppUserId { get; set; } // Foreign Key

        [Required]
        public float Rating { get; set; }

        [Required]
        public string Comment { get; set; }

        public bool Approved { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }
    }

}
