namespace EventBooking.Models.ViewModels
{
    // ViewModel for the Home page with featured events and personalized recommendations
    public class HomeViewModel
    {
        public List<Event> FeaturedEvents { get; set; } = new List<Event>();
        public List<Event> RecommendedEvents { get; set; } = new List<Event>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public bool IsAuthenticated { get; set; }
    }
}
