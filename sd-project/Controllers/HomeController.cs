using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventBooking.Data;
using EventBooking.Models;
using EventBooking.Models.ViewModels;

namespace EventBooking.Controllers
{
    // HomeController - handles the home/landing page
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: / - Display the home page with featured events and recommendations
        public async Task<IActionResult> Index()
        {
            // Get featured upcoming events
            var featuredEvents = await _context.Events
                .Include(e => e.Category)
                .Where(e => e.Date >= DateTime.Now && e.IsFeatured)
                .OrderBy(e => e.Date)
                .Take(6)
                .ToListAsync();

            // If not enough featured, fill with upcoming events
            if (featuredEvents.Count < 3)
            {
                var additionalEvents = await _context.Events
                    .Include(e => e.Category)
                    .Where(e => e.Date >= DateTime.Now && !featuredEvents.Select(f => f.Id).Contains(e.Id))
                    .OrderBy(e => e.Date)
                    .Take(6 - featuredEvents.Count)
                    .ToListAsync();
                featuredEvents.AddRange(additionalEvents);
            }

            // Get categories
            var categories = await _context.Categories
                .Include(c => c.Events)
                .ToListAsync();

            // Get recommended events based on user browsing history
            var recommendedEvents = new List<Event>();
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Get categories the user has viewed most
                    var viewedCategoryIds = await _context.UserEventHistories
                        .Where(h => h.UserId == user.Id)
                        .Include(h => h.Event)
                        .Select(h => h.Event!.CategoryId)
                        .ToListAsync();

                    if (viewedCategoryIds.Any())
                    {
                        // Get the most viewed category
                        var topCategoryIds = viewedCategoryIds
                            .GroupBy(c => c)
                            .OrderByDescending(g => g.Count())
                            .Take(3)
                            .Select(g => g.Key)
                            .ToList();

                        // Get events in those categories that user hasn't booked
                        var bookedEventIds = await _context.Bookings
                            .Where(b => b.UserId == user.Id)
                            .Select(b => b.EventId)
                            .ToListAsync();

                        recommendedEvents = await _context.Events
                            .Include(e => e.Category)
                            .Where(e => e.Date >= DateTime.Now
                                && topCategoryIds.Contains(e.CategoryId)
                                && !bookedEventIds.Contains(e.Id))
                            .OrderBy(e => e.Date)
                            .Take(4)
                            .ToListAsync();
                    }
                }
            }

            // If no personalized recommendations, show popular events
            if (!recommendedEvents.Any())
            {
                recommendedEvents = await _context.Events
                    .Include(e => e.Category)
                    .Where(e => e.Date >= DateTime.Now)
                    .OrderByDescending(e => e.Bookings.Count)
                    .Take(4)
                    .ToListAsync();
            }

            var model = new HomeViewModel
            {
                FeaturedEvents = featuredEvents,
                RecommendedEvents = recommendedEvents,
                Categories = categories,
                IsAuthenticated = User.Identity?.IsAuthenticated == true
            };

            return View(model);
        }
    }
}
