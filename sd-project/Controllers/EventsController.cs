using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventBooking.Data;
using EventBooking.Models;
using EventBooking.Models.ViewModels;

namespace EventBooking.Controllers
{
    // EventsController - handles listing, searching, filtering, and viewing event details
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Events - Display all events with search and filtering
        public async Task<IActionResult> Index(
            string? search, int? categoryId, string? city,
            DateTime? dateFrom, DateTime? dateTo,
            decimal? minPrice, decimal? maxPrice,
            bool freeOnly = false, string? sortBy = "date")
        {
            var query = _context.Events
                .Include(e => e.Category)
                .Where(e => e.Date >= DateTime.Now)
                .AsQueryable();

            // Search by title or description
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Title.Contains(search) || e.Description.Contains(search) || e.Location.Contains(search));
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId);
            }

            // Filter by city/location
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(e => e.City == city);
            }

            // Filter by date range
            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.Date >= dateFrom);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(e => e.Date <= dateTo);
            }

            // Filter by price
            if (freeOnly)
            {
                query = query.Where(e => e.Price == 0);
            }
            else
            {
                if (minPrice.HasValue)
                {
                    query = query.Where(e => e.Price >= minPrice);
                }
                if (maxPrice.HasValue)
                {
                    query = query.Where(e => e.Price <= maxPrice);
                }
            }

            // Sort
            query = sortBy switch
            {
                "price-low" => query.OrderBy(e => e.Price),
                "price-high" => query.OrderByDescending(e => e.Price),
                "name" => query.OrderBy(e => e.Title),
                _ => query.OrderBy(e => e.Date)
            };

            var categories = await _context.Categories.ToListAsync();
            var cities = await _context.Events.Select(e => e.City).Distinct().OrderBy(c => c).ToListAsync();

            var model = new EventSearchViewModel
            {
                Events = await query.ToListAsync(),
                Categories = categories,
                Cities = cities,
                SearchQuery = search,
                CategoryId = categoryId,
                City = city,
                DateFrom = dateFrom,
                DateTo = dateTo,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                FreeOnly = freeOnly,
                SortBy = sortBy
            };

            return View(model);
        }

        // GET: /Events/Details/5 - Display details for a specific event
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.TicketTypes)
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Track user browsing history for personalized feed
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Only add if not viewed recently (within last hour)
                    var recentView = await _context.UserEventHistories
                        .FirstOrDefaultAsync(h => h.UserId == user.Id && h.EventId == id
                            && h.ViewedAt > DateTime.Now.AddHours(-1));

                    if (recentView == null)
                    {
                        _context.UserEventHistories.Add(new UserEventHistory
                        {
                            UserId = user.Id,
                            EventId = id.Value,
                            ViewedAt = DateTime.Now
                        });
                        await _context.SaveChangesAsync();
                    }
                }
            }

            // Get related events (same category, different event)
            var relatedEvents = await _context.Events
                .Include(e => e.Category)
                .Where(e => e.CategoryId == eventItem.CategoryId && e.Id != eventItem.Id && e.Date >= DateTime.Now)
                .OrderBy(e => e.Date)
                .Take(3)
                .ToListAsync();

            // Check if user has registered/booked this event
            bool isRegistered = false;
            bool isSaved = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    isRegistered = await _context.Bookings
                        .AnyAsync(b => b.UserId == user.Id && b.EventId == id && b.Status != "Cancelled");
                    isSaved = await _context.SavedEvents
                        .AnyAsync(s => s.UserId == user.Id && s.EventId == id);
                }
            }

            var model = new EventDetailsViewModel
            {
                Event = eventItem,
                RelatedEvents = relatedEvents,
                TicketTypes = eventItem.TicketTypes.ToList(),
                IsUserRegistered = isRegistered,
                IsEventSaved = isSaved
            };

            return View(model);
        }
    }
}
