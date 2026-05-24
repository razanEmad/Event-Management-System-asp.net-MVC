using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventBooking.Data;
using EventBooking.Models;
using EventBooking.Models.ViewModels;

namespace EventBooking.Controllers
{
    // DashboardController - handles user dashboard showing bookings, orders, saved events
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Dashboard - Main user dashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var now = DateTime.Now;

            var upcomingBookings = await _context.Bookings
                .Include(b => b.Event).ThenInclude(e => e!.Category)
                .Include(b => b.TicketType)
                .Where(b => b.UserId == user.Id && b.Event != null && b.Event.Date >= now && b.Status != "Cancelled")
                .OrderBy(b => b.Event!.Date)
                .ToListAsync();

            var pastBookings = await _context.Bookings
                .Include(b => b.Event).ThenInclude(e => e!.Category)
                .Include(b => b.TicketType)
                .Where(b => b.UserId == user.Id && b.Event != null && b.Event.Date < now)
                .OrderByDescending(b => b.Event!.Date)
                .ToListAsync();

            var orders = await _context.Orders
                .Include(o => o.Booking).ThenInclude(b => b!.Event)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var savedEvents = await _context.SavedEvents
                .Include(s => s.Event).ThenInclude(e => e!.Category)
                .Where(s => s.UserId == user.Id)
                .OrderByDescending(s => s.SavedAt)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                User = user,
                UpcomingBookings = upcomingBookings,
                PastBookings = pastBookings,
                Orders = orders,
                SavedEvents = savedEvents,
                TotalEventsAttended = pastBookings.Count,
                UpcomingEventsCount = upcomingBookings.Count,
                TotalSpent = orders.Sum(o => o.TotalAmount)
            };

            return View(model);
        }

        // POST: /Dashboard/CancelBooking/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.TicketType)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == user.Id);

            if (booking == null) return NotFound();

            booking.Status = "Cancelled";

            // Restore ticket availability
            if (booking.Event != null)
            {
                booking.Event.AvailableTickets += booking.TicketCount;
            }
            if (booking.TicketType != null)
            {
                booking.TicketType.AvailableCount += booking.TicketCount;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction("Index");
        }

        // POST: /Dashboard/SaveEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEvent(int eventId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Check if already saved
            var existing = await _context.SavedEvents
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.EventId == eventId);

            if (existing == null)
            {
                _context.SavedEvents.Add(new SavedEvent
                {
                    UserId = user.Id,
                    EventId = eventId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        // POST: /Dashboard/UnsaveEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnsaveEvent(int eventId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var saved = await _context.SavedEvents
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.EventId == eventId);

            if (saved != null)
            {
                _context.SavedEvents.Remove(saved);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
