using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    // Payment model - tracks payment transactions for bookings
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Method { get; set; } = "CreditCard"; // CreditCard, DebitCard, PayPal, etc.

        [StringLength(200)]
        public string? TransactionId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded

        [StringLength(100)]
        public string? CardLast4 { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? PaidAt { get; set; }

        // Foreign key
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
    }
}
