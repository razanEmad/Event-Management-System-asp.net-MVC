namespace EventBooking.Models.ViewModels
{
    // ViewModel for the Event Details page including related events
    public class EventDetailsViewModel
    {
        public Event Event { get; set; } = null!;
        public List<Event> RelatedEvents { get; set; } = new List<Event>();
        public List<TicketType> TicketTypes { get; set; } = new List<TicketType>();
        public bool IsUserRegistered { get; set; }
        public bool IsEventSaved { get; set; }
    }
}
