using Microsoft.EntityFrameworkCore;

namespace BusBooking_API.Model.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {

        }
        public DbSet<User> Users { get; set; }

        public DbSet<BusRoute> BusRoutes { get; set; }

        public DbSet<Bus> Buses { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<BookingSeat> BookingSeats { get; set; }

        public DbSet<Payment> Payments { get; set; }

    }
}
