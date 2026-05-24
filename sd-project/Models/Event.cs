using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    // Event model - represents an event that users can book tickets for
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // URL or path to the event image
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date & Time")]
        public DateTime Date { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "End Date & Time")]
        public DateTime? EndDate { get; set; }

        [Required]
        [StringLength(300)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [Range(0, 10000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Available Tickets")]
        public int AvailableTickets { get; set; }

        [Display(Name = "Max Capacity")]
        [Range(0, 100000)]
        public int MaxCapacity { get; set; }

        // Recurrence Rule (e.g., "every Tuesday") - stores cron-like expression
        [StringLength(200)]
        [Display(Name = "Recurrence Rule")]
        public string? RecurrenceRule { get; set; }

        [Display(Name = "Featured Event")]
        public bool IsFeatured { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key to Category
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Navigation properties
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<TicketType> TicketTypes { get; set; } = new List<TicketType>();
    }
}
