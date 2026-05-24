using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    // Order model - represents a completed order tied to a user
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Confirmed"; // Confirmed, Cancelled, Refunded

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Foreign keys
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
    }
}
