using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    // Booking model - represents a ticket booking made by a customer
    public class Booking
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the number of tickets")]
        [Range(1, 10, ErrorMessage = "You can book between 1 and 10 tickets")]
        [Display(Name = "Number of Tickets")]
        public int TicketCount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Confirmed"; // Confirmed, Cancelled, Pending

        [StringLength(50)]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Refunded

        public DateTime BookingDate { get; set; } = DateTime.Now;

        // Foreign key to the Event
        public int EventId { get; set; }

        // Navigation property - each booking belongs to one event
        public Event? Event { get; set; }

        // Foreign key to TicketType (optional for backward compat)
        public int? TicketTypeId { get; set; }
        public TicketType? TicketType { get; set; }

        // Foreign key to User (optional - guest booking still supported)
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Navigation properties
        public Payment? Payment { get; set; }
        public Order? Order { get; set; }
    }
}
