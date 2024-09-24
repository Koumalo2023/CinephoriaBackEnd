using Microsoft.AspNetCore.Identity;

namespace CinephoriaBackEnd.Models
{
    public class AppUser : IdentityUser
    {
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Reservation> Reservations { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
