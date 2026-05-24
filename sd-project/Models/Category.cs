using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    // Category model - organizes events into browsable categories
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Slug { get; set; } = string.Empty;

        // Bootstrap icon class name (e.g., "bi-music-note-beamed")
        [StringLength(100)]
        public string Icon { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Image URL for category card
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        // Navigation property
        public List<Event> Events { get; set; } = new List<Event>();
    }
}
