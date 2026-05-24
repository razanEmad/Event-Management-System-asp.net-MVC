namespace EventBooking.Models
{
    // UserEventHistory - tracks which events a user has viewed (for personalized recommendations)
    public class UserEventHistory
    {
        public int Id { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}
