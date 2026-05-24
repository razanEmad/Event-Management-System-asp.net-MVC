using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    // ApplicationUser - extends IdentityUser with custom profile fields
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(300)]
        public string? City { get; set; }

        [StringLength(500)]
        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<SavedEvent> SavedEvents { get; set; } = new List<SavedEvent>();
        public List<UserEventHistory> BrowsingHistory { get; set; } = new List<UserEventHistory>();
    }
}
