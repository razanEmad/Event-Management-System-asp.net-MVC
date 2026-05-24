namespace EventBooking.Models.ViewModels
{
    // ViewModel for the Events listing page with search/filter support
    public class EventSearchViewModel
    {
        public List<Event> Events { get; set; } = new List<Event>();
        public List<Category> Categories { get; set; } = new List<Category>();

        // Search & Filter parameters
        public string? SearchQuery { get; set; }
        public int? CategoryId { get; set; }
        public string? City { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool FreeOnly { get; set; }
        public string? SortBy { get; set; } // date, price, name

        // For location filter dropdown
        public List<string> Cities { get; set; } = new List<string>();
    }
}
