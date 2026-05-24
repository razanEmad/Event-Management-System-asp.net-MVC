using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventBooking.Data;

namespace EventBooking.Controllers
{
    // CategoryController - handles event category browsing
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Category - Display all categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Events)
                .ToListAsync();

            return View(categories);
        }

        // GET: /Category/Details/music - Display events in a specific category
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return NotFound();

            var category = await _context.Categories
                .Include(c => c.Events)
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}
