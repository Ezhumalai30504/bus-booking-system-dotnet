using BusBooking_API.Model.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusBooking_API.Model.Dataseeder
{
    public class DataSeeder
    {
        public static async Task SeedAdminAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Role == "Admin"))
                return;

            var admin = new User
            {
                Name = "System Admin",
                Email = "admin@bus.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                Role = "Admin"
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
