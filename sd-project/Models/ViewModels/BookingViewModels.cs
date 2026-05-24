using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EventBooking.Models.ViewModels
{
    // ViewModel for the booking/checkout form with ticket type selection
    public class BookingCreateViewModel
    {
        [ValidateNever] //Do NOT validate this property
        public Event Event { get; set; } = null!;
        [ValidateNever]
        public List<TicketType> TicketTypes { get; set; } = new List<TicketType>();

        //[Required] User MUST enter a value. If empty: Please enter your name
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email")]
        //This validates email format 
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a ticket type")]
        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }

        [Required(ErrorMessage = "Please enter the number of tickets")]
        [Range(1, 10, ErrorMessage = "You can book between 1 and 10 tickets")]
        [Display(Name = "Number of Tickets")]
        public int TicketCount { get; set; } = 1;

        public int EventId { get; set; }
    }

    // ViewModel for the payment/checkout page
    public class CheckoutViewModel
    {
        [ValidateNever]
        public Booking Booking { get; set; } = null!;
        [ValidateNever]
        public Event Event { get; set; } = null!;
        [ValidateNever]
        public TicketType TicketType { get; set; } = null!;
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Card number is required")]
        [StringLength(19)]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cardholder name is required")]
        [StringLength(100)]
        [Display(Name = "Cardholder Name")]
        public string CardholderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiry date is required")]
        [StringLength(5)]
        [Display(Name = "Expiry (MM/YY)")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV is required")]
        [StringLength(4)]
        public string CVV { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "CreditCard";
    }
}
