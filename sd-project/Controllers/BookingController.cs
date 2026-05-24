using EventBooking.Data;
using EventBooking.Models;
using EventBooking.Models.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;

namespace EventBooking.Controllers
{
    // BookingController - handles ticket booking with ticket type selection and payment
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Booking/Create/5 - Show the booking form for a specific event
        //It only shows the booking page 
        //1. Validate input 2. Get event from database 3. Check availability
        //4. Prepare ViewModel (for UI only) 5. If user is logged in
        //6. Prevent duplicate booking 7. Auto-fill form 8. Return View
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Events //_context.Events like select * from Events
                .Include(e => e.Category) //Also load the related Category data for this event
                .Include(e => e.TicketTypes) //Event ->CategoryId -> Category 
                .FirstOrDefaultAsync(e => e.Id == id); //Give me the first event where
                                                       //Id matches this id

            if (eventItem == null) return NotFound();

            // Check if sold out
            if (eventItem.AvailableTickets <= 0)
            {
                TempData["Error"] = "Sorry, this event is sold out!";
                //TempData is specifically designed for: passing data from one request to
                //the NEXT request especially after redirects.
                return RedirectToAction("Details", "Events", new { id });
            }

            // Pre-fill user info if logged in
            //Preparing data to send to the View
            //This code creates a temporary object in memory to send structured
            //data to the View — not to the database
            var model = new BookingCreateViewModel
            {
                EventId = eventItem.Id,
                Event = eventItem,
                TicketTypes = eventItem.TicketTypes.Where(t => t.AvailableCount > 0).ToList()
            };
            //User: It represents the current HTTP user (the person using the website)
            //User.Identity This contains information about the user’s authentication status.
            if (User.Identity?.IsAuthenticated == true) //Is the user logged in?
                    //'?':If Identity is null, don’t crash — just return null
            {
                //This loads the full user object from Identity system
                var user = await _userManager.GetUserAsync(User); 
                
                if (user != null)
                {
                    // Prevent duplicate registration
                    var existingBooking = await _context.Bookings
                        .AnyAsync(b => b.UserId == user.Id && b.EventId == id && b.Status != "Cancelled");

                    if (existingBooking)
                    {
                        TempData["Error"] = "You have already registered for this event!";
                        return RedirectToAction("Details", "Events", new { id });
                    }

                    //Pre-fill booking form with user data
                    model.CustomerName = user.FullName;
                    model.Email = user.Email ?? ""; //If email is null, use empty string
                }
            }

            return View(model);
        }

        // POST: /Booking/Create - Process the booking form submission
        //This is where the booking is actually saved
        //[]:They add special behavior to your action method
        [HttpPost] //POST request
        [ValidateAntiForgeryToken]// This is a security feature 
        //ASP.NET generates a hidden secret token inside the form:
        //<input type = "hidden"...> Then when form is submitted:
        //server checks the token
        //if token is invalid -> reject request

        //public async Task<IActionResult>:This controller action runs asynchronously
        // and will eventually return a response/page/result
        //Task: asynchronous programming
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            var eventItem = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == model.EventId);

            if (eventItem == null) return NotFound();

            // Validate ticket type
            var ticketType = eventItem.TicketTypes.FirstOrDefault(t => t.Id == model.TicketTypeId);
            if (ticketType == null)
            {
                //ModelState stores: form values, validation errors
                //ASP.NET MVC uses it to know: Is the submitted form valid?
                //AddModelError(): Add a validation error to the form
                ModelState.AddModelError("TicketTypeId", "Please select a valid ticket type.");
            }
            else if (model.TicketCount > ticketType.AvailableCount)
            {
                ModelState.AddModelError("TicketCount",
                    $"Only {ticketType.AvailableCount} tickets of type '{ticketType.Name}' are available.");
            }

            // Check overall event capacity
            if (model.TicketCount > eventItem.AvailableTickets)
            {
                ModelState.AddModelError("TicketCount",
                    $"Only {eventItem.AvailableTickets} tickets are available for this event.");
            }

            //This is the main booking logic.
            //It runs after the user submits the booking form.
            //Validate form -> create booking ->reduce tickets -> create order/payment
            //-> save everything -> redirect to success page
            if (ModelState.IsValid && ticketType != null)
            {
                var totalPrice = ticketType.Price * model.TicketCount;

                // Create the booking
                var booking = new Booking
                {
                    CustomerName = model.CustomerName,
                    Email = model.Email,
                    TicketCount = model.TicketCount,
                    EventId = model.EventId,
                    TicketTypeId = model.TicketTypeId,
                    TotalPrice = totalPrice,
                    Status = "Confirmed",
                    PaymentStatus = totalPrice > 0 ? "Paid" : "Free",
                    BookingDate = DateTime.Now
                };

                // Associate with user if logged in
                if (User.Identity?.IsAuthenticated == true)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        booking.UserId = user.Id;
                    }
                }

                _context.Bookings.Add(booking);

                // Reduce available tickets
                ticketType.AvailableCount -= model.TicketCount;
                eventItem.AvailableTickets -= model.TicketCount;

                await _context.SaveChangesAsync();

                // Create order
                var order = new Order
                {
                    OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{booking.Id:D4}",
                    TotalAmount = totalPrice,
                    Status = "Confirmed",
                    BookingId = booking.Id,
                    UserId = booking.UserId
                };
                _context.Orders.Add(order);

                // If there's a price, create a dummy payment to satisfy relationships
                if (totalPrice > 0)
                {
                    var payment = new Payment
                    {
                        BookingId = booking.Id,
                        Amount = totalPrice,
                        Method = "Direct Checkout",
                        TransactionId = $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{booking.Id}",
                        Status = "Completed",
                        CardLast4 = "****",
                        PaidAt = DateTime.Now
                    };
                    _context.Payments.Add(payment);
                }

                await _context.SaveChangesAsync();

                // Redirect directly to Success, bypassing Checkout
                return RedirectToAction("Success", new { id = booking.Id });
            }

            // If validation fails, show the form again
            model.Event = eventItem;
            model.TicketTypes = eventItem.TicketTypes.Where(t => t.AvailableCount > 0).ToList();
            return View(model);
        }

        // GET: /Booking/Checkout?bookingId=5 - Show payment page
        public async Task<IActionResult> Checkout(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event).ThenInclude(e => e!.Category)
                .Include(b => b.TicketType)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return NotFound();

            var model = new CheckoutViewModel
            {
                Booking = booking,
                Event = booking.Event!,
                TicketType = booking.TicketType!,
                TotalAmount = booking.TotalPrice,
                PaymentMethod = "CreditCard"
            };

            return View(model);
        }

        // POST: /Booking/ProcessPayment - Process the payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(CheckoutViewModel model, int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event).ThenInclude(e => e!.Category)
                .Include(b => b.TicketType)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return NotFound();

            // Simulate payment processing (in production, integrate real payment gateway)
            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                Method = model.PaymentMethod,
                TransactionId = $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{booking.Id}",
                Status = "Completed",
                CardLast4 = model.CardNumber.Length >= 4
                    ? model.CardNumber.Substring(model.CardNumber.Length - 4)
                    : "****",
                PaidAt = DateTime.Now
            };

            _context.Payments.Add(payment);

            // Update booking status
            booking.Status = "Confirmed";
            booking.PaymentStatus = "Paid";

            // Create order
            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{booking.Id:D4}",
                TotalAmount = booking.TotalPrice,
                Status = "Confirmed",
                BookingId = booking.Id,
                UserId = booking.UserId
            };
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { id = booking.Id });
        }

        // GET: /Booking/Success/5 - Show success message after booking
        public async Task<IActionResult> Success(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event).ThenInclude(e => e!.Category) 
                //e!. :I KNOW this object is not null, trust me
                .Include(b => b.TicketType)
                .Include(b => b.Payment)
                .Include(b => b.Order)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking); //ASP.NET MVC automatically looks for a file named:
            //Views / Booking / Success.cshtml
            //Controller name = BookingController, Action name = Success 
            //What gets sent to the View? The object: booking 
        }
    }
}
