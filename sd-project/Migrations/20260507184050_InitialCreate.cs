using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventBooking.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailableTickets = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketCount = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "AvailableTickets", "Date", "Description", "ImageUrl", "Location", "Price", "Title" },
                values: new object[,]
                {
                    { 1, 500, new DateTime(2026, 7, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), "Join us for an unforgettable summer music festival featuring top artists from around the world. Enjoy live performances, food trucks, and an amazing atmosphere under the stars. This three-day event will feature multiple stages with genres ranging from rock and pop to electronic and indie music.", "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=800&h=500&fit=crop", "Central Park, New York", 75.00m, "Summer Music Festival" },
                    { 2, 300, new DateTime(2026, 8, 20, 9, 0, 0, 0, DateTimeKind.Unspecified), "Explore the latest in technology and innovation at our annual conference. Hear from industry leaders, attend hands-on workshops, and network with professionals. Topics include AI, cloud computing, cybersecurity, and the future of software development.", "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=500&fit=crop", "Convention Center, San Francisco", 120.00m, "Tech Innovation Conference" },
                    { 3, 200, new DateTime(2026, 9, 10, 19, 0, 0, 0, DateTimeKind.Unspecified), "Indulge in an evening of exquisite cuisine and fine wines from renowned chefs and vineyards. This elegant gala features a five-course tasting menu paired with premium wines, live jazz music, and an exclusive cooking demonstration.", "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800&h=500&fit=crop", "Grand Ballroom, Chicago", 95.00m, "Food & Wine Tasting Gala" },
                    { 4, 1000, new DateTime(2026, 10, 5, 7, 0, 0, 0, DateTimeKind.Unspecified), "Challenge yourself in our annual city marathon! Whether you're a seasoned runner or a beginner, join thousands of participants running through scenic city routes. The event includes a full marathon, half marathon, and a fun 5K run for families.", "https://images.unsplash.com/photo-1532444458054-01a7dd3e9fca?w=800&h=500&fit=crop", "Downtown, Boston", 45.00m, "Marathon City Run" },
                    { 5, 150, new DateTime(2026, 11, 12, 10, 0, 0, 0, DateTimeKind.Unspecified), "Discover stunning works of art and photography from emerging and established artists. This curated exhibition showcases contemporary pieces in painting, sculpture, digital art, and photography. Meet the artists and enjoy guided tours throughout the day.", "https://images.unsplash.com/photo-1531243269054-5ebf6f34081e?w=800&h=500&fit=crop", "Modern Art Gallery, Los Angeles", 30.00m, "Art & Photography Exhibition" },
                    { 6, 250, new DateTime(2026, 12, 1, 20, 0, 0, 0, DateTimeKind.Unspecified), "Get ready for a night of non-stop laughter with some of the funniest comedians in the business. Our Comedy Night Live event features stand-up performances, improv shows, and audience participation segments that will keep you entertained all evening.", "https://images.unsplash.com/photo-1527224538127-2104bb71c51b?w=800&h=500&fit=crop", "Laugh Factory, Las Vegas", 55.00m, "Comedy Night Live" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EventId",
                table: "Bookings",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
