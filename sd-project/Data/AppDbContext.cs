using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventBooking.Models;

namespace EventBooking.Data
{
    // Database context - this is the bridge between our code and the database
    // Now extends IdentityDbContext for user authentication
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tables in our database
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SavedEvent> SavedEvents { get; set; }
        public DbSet<UserEventHistory> UserEventHistories { get; set; }

        // Seed sample data so the website has content on first run
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            // Event -> Category
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> Event
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId);

            // Booking -> TicketType
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TicketType)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> User
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // TicketType -> Event
            modelBuilder.Entity<TicketType>()
                .HasOne(t => t.Event)
                .WithMany(e => e.TicketTypes)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment -> Booking (one-to-one)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId);

            // Order -> Booking (one-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Booking)
                .WithOne(b => b.Order)
                .HasForeignKey<Order>(o => o.BookingId);

            // Order -> User
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // SavedEvent -> User
            modelBuilder.Entity<SavedEvent>()
                .HasOne(s => s.User)
                .WithMany(u => u.SavedEvents)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // SavedEvent -> Event
            modelBuilder.Entity<SavedEvent>()
                .HasOne(s => s.Event)
                .WithMany()
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserEventHistory -> User
            modelBuilder.Entity<UserEventHistory>()
                .HasOne(h => h.User)
                .WithMany(u => u.BrowsingHistory)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserEventHistory -> Event
            modelBuilder.Entity<UserEventHistory>()
                .HasOne(h => h.Event)
                .WithMany()
                .HasForeignKey(h => h.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Set decimal precision
            modelBuilder.Entity<Event>()
                .Property(e => e.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TicketType>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            // Unique constraint on SavedEvent (user can only save an event once)
            modelBuilder.Entity<SavedEvent>()
                .HasIndex(s => new { s.UserId, s.EventId })
                .IsUnique();

            // ===== SEED DATA =====

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Music",
                    Slug = "music",
                    Icon = "bi-music-note-beamed",
                    Description = "Concerts, festivals, and live music performances",
                    ImageUrl = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=600&h=400&fit=crop"
                },
                new Category
                {
                    Id = 2,
                    Name = "Technology",
                    Slug = "technology",
                    Icon = "bi-cpu",
                    Description = "Tech conferences, hackathons, and innovation summits",
                    ImageUrl = "https://images.unsplash.com/photo-1518770660439-4636190af475?w=600&h=400&fit=crop"
                },
                new Category
                {
                    Id = 3,
                    Name = "Sports",
                    Slug = "sports",
                    Icon = "bi-trophy",
                    Description = "Marathons, tournaments, and sporting events",
                    ImageUrl = "https://images.unsplash.com/photo-1461896836934-bd45ba24e4c7?w=600&h=400&fit=crop"
                },
                new Category
                {
                    Id = 4,
                    Name = "Food & Drink",
                    Slug = "food-drink",
                    Icon = "bi-cup-straw",
                    Description = "Food festivals, wine tastings, and culinary experiences",
                    ImageUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=600&h=400&fit=crop"
                },
                new Category
                {
                    Id = 5,
                    Name = "Arts & Theater",
                    Slug = "arts-theater",
                    Icon = "bi-palette",
                    Description = "Art exhibitions, theater performances, and cultural shows",
                    ImageUrl = "https://images.unsplash.com/photo-1531243269054-5ebf6f34081e?w=600&h=400&fit=crop"
                },
                new Category
                {
                    Id = 6,
                    Name = "Workshops",
                    Slug = "workshops",
                    Icon = "bi-tools",
                    Description = "Hands-on workshops, training sessions, and skill-building events",
                    ImageUrl = "https://images.unsplash.com/photo-1552664730-d307ca884978?w=600&h=400&fit=crop"
                },
                new Category
                {
                    Id = 7,
                    Name = "Comedy",
                    Slug = "comedy",
                    Icon = "bi-emoji-laughing",
                    Description = "Stand-up comedy shows, improv nights, and comedy festivals",
                    ImageUrl = "https://images.unsplash.com/photo-1527224538127-2104bb71c51b?w=600&h=400&fit=crop"
                },
                new Category
                {
                    Id = 8,
                    Name = "Conferences",
                    Slug = "conferences",
                    Icon = "bi-people",
                    Description = "Professional conferences, seminars, and networking events",
                    ImageUrl = "https://images.unsplash.com/photo-1505373877841-8d25f7d46678?w=600&h=400&fit=crop"
                }
            );

            // Seed Events with categories
            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Summer Music Festival",
                    Description = "Join us for an unforgettable summer music festival featuring top artists from around the world. Enjoy live performances, food trucks, and an amazing atmosphere under the stars. This three-day event will feature multiple stages with genres ranging from rock and pop to electronic and indie music.",
                    ImageUrl = "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 7, 15, 18, 0, 0),
                    EndDate = new DateTime(2026, 7, 17, 23, 0, 0),
                    Location = "Central Park, New York",
                    City = "New York",
                    Price = 75.00m,
                    AvailableTickets = 500,
                    MaxCapacity = 500,
                    CategoryId = 1,
                    IsFeatured = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 2,
                    Title = "Tech Innovation Conference",
                    Description = "Explore the latest in technology and innovation at our annual conference. Hear from industry leaders, attend hands-on workshops, and network with professionals. Topics include AI, cloud computing, cybersecurity, and the future of software development.",
                    ImageUrl = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 8, 20, 9, 0, 0),
                    EndDate = new DateTime(2026, 8, 21, 17, 0, 0),
                    Location = "Convention Center, San Francisco",
                    City = "San Francisco",
                    Price = 120.00m,
                    AvailableTickets = 300,
                    MaxCapacity = 300,
                    CategoryId = 2,
                    IsFeatured = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 3,
                    Title = "Food & Wine Tasting Gala",
                    Description = "Indulge in an evening of exquisite cuisine and fine wines from renowned chefs and vineyards. This elegant gala features a five-course tasting menu paired with premium wines, live jazz music, and an exclusive cooking demonstration.",
                    ImageUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 9, 10, 19, 0, 0),
                    EndDate = new DateTime(2026, 9, 10, 23, 0, 0),
                    Location = "Grand Ballroom, Chicago",
                    City = "Chicago",
                    Price = 95.00m,
                    AvailableTickets = 200,
                    MaxCapacity = 200,
                    CategoryId = 4,
                    IsFeatured = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 4,
                    Title = "Marathon City Run",
                    Description = "Challenge yourself in our annual city marathon! Whether you're a seasoned runner or a beginner, join thousands of participants running through scenic city routes. The event includes a full marathon, half marathon, and a fun 5K run for families.",
                    ImageUrl = "https://images.unsplash.com/photo-1532444458054-01a7dd3e9fca?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 10, 5, 7, 0, 0),
                    EndDate = new DateTime(2026, 10, 5, 14, 0, 0),
                    Location = "Downtown, Boston",
                    City = "Boston",
                    Price = 45.00m,
                    AvailableTickets = 1000,
                    MaxCapacity = 1000,
                    CategoryId = 3,
                    IsFeatured = false,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 5,
                    Title = "Art & Photography Exhibition",
                    Description = "Discover stunning works of art and photography from emerging and established artists. This curated exhibition showcases contemporary pieces in painting, sculpture, digital art, and photography. Meet the artists and enjoy guided tours throughout the day.",
                    ImageUrl = "https://images.unsplash.com/photo-1531243269054-5ebf6f34081e?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 11, 12, 10, 0, 0),
                    EndDate = new DateTime(2026, 11, 14, 18, 0, 0),
                    Location = "Modern Art Gallery, Los Angeles",
                    City = "Los Angeles",
                    Price = 30.00m,
                    AvailableTickets = 150,
                    MaxCapacity = 150,
                    CategoryId = 5,
                    IsFeatured = false,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 6,
                    Title = "Comedy Night Live",
                    Description = "Get ready for a night of non-stop laughter with some of the funniest comedians in the business. Our Comedy Night Live event features stand-up performances, improv shows, and audience participation segments that will keep you entertained all evening.",
                    ImageUrl = "https://images.unsplash.com/photo-1527224538127-2104bb71c51b?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 12, 1, 20, 0, 0),
                    EndDate = new DateTime(2026, 12, 1, 23, 0, 0),
                    Location = "Laugh Factory, Las Vegas",
                    City = "Las Vegas",
                    Price = 55.00m,
                    AvailableTickets = 250,
                    MaxCapacity = 250,
                    CategoryId = 7,
                    IsFeatured = false,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 7,
                    Title = "Jazz Night at Blue Note",
                    Description = "Experience an intimate evening of world-class jazz at the legendary Blue Note venue. Featuring acclaimed jazz musicians performing classic standards and original compositions. Enjoy cocktails and small plates while immersing yourself in smooth jazz melodies.",
                    ImageUrl = "https://images.unsplash.com/photo-1511192336575-5a79af67a629?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 8, 5, 20, 0, 0),
                    EndDate = new DateTime(2026, 8, 5, 23, 30, 0),
                    Location = "Blue Note Jazz Club, New York",
                    City = "New York",
                    Price = 65.00m,
                    AvailableTickets = 120,
                    MaxCapacity = 120,
                    CategoryId = 1,
                    IsFeatured = false,
                    RecurrenceRule = "Every Tuesday",
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 8,
                    Title = "Web Development Workshop",
                    Description = "A hands-on workshop for aspiring web developers. Learn HTML, CSS, JavaScript, and modern frameworks from experienced instructors. Build a real project by the end of the day and leave with new skills to advance your career in tech.",
                    ImageUrl = "https://images.unsplash.com/photo-1552664730-d307ca884978?w=800&h=400&fit=crop",
                    Date = new DateTime(2026, 9, 1, 9, 0, 0),
                    EndDate = new DateTime(2026, 9, 1, 17, 0, 0),
                    Location = "Tech Hub, San Francisco",
                    City = "San Francisco",
                    Price = 0.00m,
                    AvailableTickets = 50,
                    MaxCapacity = 50,
                    CategoryId = 6,
                    IsFeatured = false,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 9,
                    Title = "Business Leadership Summit",
                    Description = "Join top business leaders and entrepreneurs at this premier conference. Sessions cover leadership strategies, market trends, digital transformation, and sustainable business practices. Network with 500+ professionals and gain actionable insights.",
                    ImageUrl = "https://images.unsplash.com/photo-1505373877841-8d25f7d46678?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 10, 15, 8, 0, 0),
                    EndDate = new DateTime(2026, 10, 16, 18, 0, 0),
                    Location = "Hilton Conference Center, Chicago",
                    City = "Chicago",
                    Price = 250.00m,
                    AvailableTickets = 400,
                    MaxCapacity = 400,
                    CategoryId = 8,
                    IsFeatured = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Event
                {
                    Id = 10,
                    Title = "Rock Concert: The Amplifiers",
                    Description = "The Amplifiers are back with their world tour! Don't miss their electrifying live performance featuring hits from their latest album. Special guest opening acts and an incredible light show make this a must-attend event for rock music fans.",
                    ImageUrl = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3?w=800&h=500&fit=crop",
                    Date = new DateTime(2026, 11, 20, 19, 0, 0),
                    EndDate = new DateTime(2026, 11, 20, 23, 0, 0),
                    Location = "Madison Square Garden, New York",
                    City = "New York",
                    Price = 150.00m,
                    AvailableTickets = 800,
                    MaxCapacity = 800,
                    CategoryId = 1,
                    IsFeatured = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                }
            );

            // Seed Ticket Types for each event
            modelBuilder.Entity<TicketType>().HasData(
                // Event 1 - Summer Music Festival
                new TicketType { Id = 1, EventId = 1, Name = "Standard", Price = 75.00m, Quantity = 350, AvailableCount = 350, Description = "General admission" },
                new TicketType { Id = 2, EventId = 1, Name = "VIP", Price = 150.00m, Quantity = 100, AvailableCount = 100, Description = "Front row access + backstage pass" },
                new TicketType { Id = 3, EventId = 1, Name = "Student", Price = 45.00m, Quantity = 50, AvailableCount = 50, Description = "Valid student ID required" },
                // Event 2 - Tech Conference
                new TicketType { Id = 4, EventId = 2, Name = "Standard", Price = 120.00m, Quantity = 200, AvailableCount = 200, Description = "All sessions access" },
                new TicketType { Id = 5, EventId = 2, Name = "VIP", Price = 250.00m, Quantity = 50, AvailableCount = 50, Description = "All sessions + speaker dinner + networking lounge" },
                new TicketType { Id = 6, EventId = 2, Name = "Student", Price = 60.00m, Quantity = 50, AvailableCount = 50, Description = "Valid student ID required" },
                // Event 3 - Food & Wine Gala
                new TicketType { Id = 7, EventId = 3, Name = "Standard", Price = 95.00m, Quantity = 150, AvailableCount = 150, Description = "Full tasting menu" },
                new TicketType { Id = 8, EventId = 3, Name = "VIP", Price = 180.00m, Quantity = 50, AvailableCount = 50, Description = "Premium wines + chef meet & greet" },
                // Event 4 - Marathon
                new TicketType { Id = 9, EventId = 4, Name = "Full Marathon", Price = 45.00m, Quantity = 500, AvailableCount = 500, Description = "42.2km full marathon" },
                new TicketType { Id = 10, EventId = 4, Name = "Half Marathon", Price = 30.00m, Quantity = 300, AvailableCount = 300, Description = "21.1km half marathon" },
                new TicketType { Id = 11, EventId = 4, Name = "5K Fun Run", Price = 15.00m, Quantity = 200, AvailableCount = 200, Description = "Family-friendly 5K" },
                // Event 5 - Art Exhibition
                new TicketType { Id = 12, EventId = 5, Name = "Standard", Price = 30.00m, Quantity = 100, AvailableCount = 100, Description = "General admission" },
                new TicketType { Id = 13, EventId = 5, Name = "VIP", Price = 60.00m, Quantity = 50, AvailableCount = 50, Description = "Guided tour + artist meet" },
                // Event 6 - Comedy Night
                new TicketType { Id = 14, EventId = 6, Name = "Standard", Price = 55.00m, Quantity = 200, AvailableCount = 200, Description = "General seating" },
                new TicketType { Id = 15, EventId = 6, Name = "VIP", Price = 100.00m, Quantity = 50, AvailableCount = 50, Description = "Front row + meet comedians" },
                // Event 7 - Jazz Night
                new TicketType { Id = 16, EventId = 7, Name = "Standard", Price = 65.00m, Quantity = 80, AvailableCount = 80, Description = "Table seating" },
                new TicketType { Id = 17, EventId = 7, Name = "VIP", Price = 120.00m, Quantity = 40, AvailableCount = 40, Description = "Premium table + complimentary drinks" },
                // Event 8 - Workshop (Free)
                new TicketType { Id = 18, EventId = 8, Name = "Free Registration", Price = 0.00m, Quantity = 50, AvailableCount = 50, Description = "Free workshop seat" },
                // Event 9 - Business Summit
                new TicketType { Id = 19, EventId = 9, Name = "Standard", Price = 250.00m, Quantity = 300, AvailableCount = 300, Description = "All sessions" },
                new TicketType { Id = 20, EventId = 9, Name = "VIP", Price = 500.00m, Quantity = 100, AvailableCount = 100, Description = "All sessions + executive lounge + keynote dinner" },
                // Event 10 - Rock Concert
                new TicketType { Id = 21, EventId = 10, Name = "Standard", Price = 150.00m, Quantity = 500, AvailableCount = 500, Description = "General admission" },
                new TicketType { Id = 22, EventId = 10, Name = "VIP", Price = 300.00m, Quantity = 200, AvailableCount = 200, Description = "Floor access + exclusive merch" },
                new TicketType { Id = 23, EventId = 10, Name = "Student", Price = 90.00m, Quantity = 100, AvailableCount = 100, Description = "Valid student ID required" }
            );
        }
    }
}
