namespace EventBooking.Models.ViewModels
{
    // ViewModel for user dashboard showing upcoming/past events and order history
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<Booking> UpcomingBookings { get; set; } = new List<Booking>();
        public List<Booking> PastBookings { get; set; } = new List<Booking>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<SavedEvent> SavedEvents { get; set; } = new List<SavedEvent>();
        public int TotalEventsAttended { get; set; }
        public int UpcomingEventsCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
