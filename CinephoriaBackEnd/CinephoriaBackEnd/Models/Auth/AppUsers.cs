using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaBackEnd.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Role { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
