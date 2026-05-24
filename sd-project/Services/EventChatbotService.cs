using EventBooking.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventBooking.Services
{
    public class EventChatbotService : IChatbotService
    {
        private readonly AppDbContext _context;

        public EventChatbotService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            var msg = userMessage.ToLower().Trim();

            // Simple intent recognition
            if (msg.Contains("hello") || msg.Contains("hi") || msg.Contains("مرحبا") || msg.Contains("اهلا"))
            {
                return "Hello! I am your EventBooking AI Assistant 🤖. How can I help you today? You can ask me about upcoming events, how to book, or ticket prices.";
            }

            if (msg.Contains("how to book") || msg.Contains("how can i book") || msg.Contains("حجز"))
            {
                return "To book an event, simply go to the 'Events' page, select an event you like, and click 'Book Ticket'. You will need to create an account or log in first.";
            }

            if (msg.Contains("upcoming") || msg.Contains("what events") || msg.Contains("فعاليات"))
            {
                var upcomingEvents = await _context.Events
                    .Where(e => e.Date > DateTime.Now)
                    .OrderBy(e => e.Date)
                    .Take(3)
                    .Select(e => e.Title)
                    .ToListAsync();

                if (upcomingEvents.Any())
                {
                    return $"We have some great upcoming events! Including: {string.Join(", ", upcomingEvents)}. Go to the Events page to see more details!";
                }
                return "We currently don't have any upcoming events, but check back soon!";
            }

            if (msg.Contains("contact") || msg.Contains("support") || msg.Contains("help") || msg.Contains("تواصل"))
            {
                return "You can reach our support team at info@eventbooking.com or call us at +1 (555) 123-4567.";
            }

            if (msg.Contains("price") || msg.Contains("cost") || msg.Contains("سعر"))
            {
                return "Ticket prices vary depending on the event and ticket type (e.g., VIP, General Admission). You can find specific pricing on each event's details page.";
            }
            
            if (msg.Contains("thank you") || msg.Contains("thanks") || msg.Contains("شكرا"))
            {
                return "You're very welcome! Have a great day!";
            }

            // Fallback
            return "I'm sorry, I didn't quite understand that. You can ask me about 'upcoming events', 'how to book tickets', or 'contact support'. (If you have an OpenAI/Gemini API key, you can upgrade my brain in the source code!)";
        }
    }
}
