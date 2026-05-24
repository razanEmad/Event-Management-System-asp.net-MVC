using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models.ViewModels
{
    // ViewModel for admin event create/edit forms
    public class AdminEventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date & Time")]
        public DateTime Date { get; set; } = DateTime.Now.AddDays(7);

        [DataType(DataType.DateTime)]
        [Display(Name = "End Date & Time")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(300)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Recurrence Rule")]
        [StringLength(200)]
        public string? RecurrenceRule { get; set; }

        [Display(Name = "Featured Event")]
        public bool IsFeatured { get; set; }

        // Ticket types for the event
        public List<TicketTypeInput> TicketTypes { get; set; } = new List<TicketTypeInput>();

        // For dropdown
        public List<Category> Categories { get; set; } = new List<Category>();
    }

    // Input model for ticket type in admin form
    public class TicketTypeInput
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "Standard";

        [Required]
        [Range(0, 100000)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Quantity { get; set; } = 100;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
