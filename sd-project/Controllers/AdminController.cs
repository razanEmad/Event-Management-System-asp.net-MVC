using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventBooking.Data;
using EventBooking.Models;
using EventBooking.Models.ViewModels;

namespace EventBooking.Controllers
{
    // AdminController - handles event management for administrators
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Admin - Admin dashboard
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalEvents = await _context.Events.CountAsync();
            ViewBag.TotalBookings = await _context.Bookings.CountAsync();
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalRevenue = await _context.Bookings
                .Where(b => b.PaymentStatus == "Paid")
                .SumAsync(b => b.TotalPrice);
            ViewBag.RecentBookings = await _context.Bookings
                .Include(b => b.Event)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            return View();
        }

        // GET: /Admin/Events - List all events with search/filter
        public async Task<IActionResult> Events(string? search, int? categoryId, string? city)
        {
            var query = _context.Events.Include(e => e.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Title.Contains(search) || e.Description.Contains(search));
                ViewBag.Search = search;
            }

            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId);
                ViewBag.CategoryId = categoryId;
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(e => e.City == city);
                ViewBag.City = city;
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Cities = await _context.Events.Select(e => e.City).Distinct().ToListAsync();

            var events = await query.OrderByDescending(e => e.Date).ToListAsync();
            return View(events);
        }

        // GET: /Admin/CreateEvent
        public async Task<IActionResult> CreateEvent()
        {
            var model = new AdminEventViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                TicketTypes = new List<TicketTypeInput>
                {
                    new TicketTypeInput { Name = "Standard", Price = 50, Quantity = 100 }
                }
            };
            return View(model);
        }

        // POST: /Admin/CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(AdminEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var eventItem = new Event
                {
                    Title = model.Title,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    Date = model.Date,
                    EndDate = model.EndDate,
                    Location = model.Location,
                    City = model.City,
                    CategoryId = model.CategoryId,
                    RecurrenceRule = model.RecurrenceRule,
                    IsFeatured = model.IsFeatured,
                    Price = model.TicketTypes.Any() ? model.TicketTypes.Min(t => t.Price) : 0,
                    MaxCapacity = model.TicketTypes.Sum(t => t.Quantity),
                    AvailableTickets = model.TicketTypes.Sum(t => t.Quantity),
                    CreatedAt = DateTime.Now
                };

                _context.Events.Add(eventItem);
                await _context.SaveChangesAsync();

                // Add ticket types
                foreach (var tt in model.TicketTypes)
                {
                    _context.TicketTypes.Add(new TicketType
                    {
                        EventId = eventItem.Id,
                        Name = tt.Name,
                        Price = tt.Price,
                        Quantity = tt.Quantity,
                        AvailableCount = tt.Quantity,
                        Description = tt.Description
                    });
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Events");
            }

            model.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }

        // GET: /Admin/EditEvent/5
        public async Task<IActionResult> EditEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();

            var model = new AdminEventViewModel
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                ImageUrl = eventItem.ImageUrl,
                Date = eventItem.Date,
                EndDate = eventItem.EndDate,
                Location = eventItem.Location,
                City = eventItem.City,
                CategoryId = eventItem.CategoryId,
                RecurrenceRule = eventItem.RecurrenceRule,
                IsFeatured = eventItem.IsFeatured,
                Categories = await _context.Categories.ToListAsync(),
                TicketTypes = eventItem.TicketTypes.Select(t => new TicketTypeInput
                {
                    Name = t.Name,
                    Price = t.Price,
                    Quantity = t.Quantity,
                    Description = t.Description
                }).ToList()
            };

            return View(model);
        }

        // POST: /Admin/EditEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, AdminEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var eventItem = await _context.Events
                    .Include(e => e.TicketTypes)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (eventItem == null) return NotFound();

                eventItem.Title = model.Title;
                eventItem.Description = model.Description;
                eventItem.ImageUrl = model.ImageUrl;
                eventItem.Date = model.Date;
                eventItem.EndDate = model.EndDate;
                eventItem.Location = model.Location;
                eventItem.City = model.City;
                eventItem.CategoryId = model.CategoryId;
                eventItem.RecurrenceRule = model.RecurrenceRule;
                eventItem.IsFeatured = model.IsFeatured;
                eventItem.Price = model.TicketTypes.Any() ? model.TicketTypes.Min(t => t.Price) : 0;

                // Update ticket types: remove old, add new
                _context.TicketTypes.RemoveRange(eventItem.TicketTypes);
                foreach (var tt in model.TicketTypes)
                {
                    _context.TicketTypes.Add(new TicketType
                    {
                        EventId = eventItem.Id,
                        Name = tt.Name,
                        Price = tt.Price,
                        Quantity = tt.Quantity,
                        AvailableCount = tt.Quantity,
                        Description = tt.Description
                    });
                }

                eventItem.MaxCapacity = model.TicketTypes.Sum(t => t.Quantity);
                eventItem.AvailableTickets = model.TicketTypes.Sum(t => t.Quantity);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Events");
            }

            model.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }

        // POST: /Admin/DeleteEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();

            _context.TicketTypes.RemoveRange(eventItem.TicketTypes);
            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Event deleted successfully!";
            return RedirectToAction("Events");
        }

        // GET: /Admin/EventDetails/5
        public async Task<IActionResult> EventDetails(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.TicketTypes)
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: /Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Event)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /Admin/ToggleUserStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Ensure lockout is enabled
            await _userManager.SetLockoutEnabledAsync(user, true);

            var isLocked = await _userManager.IsLockedOutAsync(user);
            if (isLocked)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"User {user.Email} has been enabled.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["Success"] = $"User {user.Email} has been disabled.";
            }

            return RedirectToAction("Users");
        }
    }
}
