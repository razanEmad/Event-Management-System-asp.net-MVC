namespace EventBooking.Models
{
    // SavedEvent - tracks events saved/bookmarked by a user
    public class SavedEvent
    {
        public int Id { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}
