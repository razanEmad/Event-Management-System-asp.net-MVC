using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    // TicketType model - represents different ticket tiers for an event (VIP, Standard, Student)
    public class TicketType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // e.g., "VIP", "Standard", "Student"

        [Required]
        [Range(0, 100000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Total Quantity")]
        public int Quantity { get; set; }

        [Range(0, 100000)]
        [Display(Name = "Available Count")]
        public int AvailableCount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; } // e.g., "Includes backstage access"

        // Foreign key
        public int EventId { get; set; }
        public Event? Event { get; set; }

        // Navigation
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
