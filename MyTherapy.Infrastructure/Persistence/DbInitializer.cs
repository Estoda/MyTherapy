using MyTherapy.Domain.Entities;
using MyTherapy.Domain.Enums;
using BCrypt.Net;

namespace MyTherapy.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void SeedAdmin(AppDbContext context)
    {
        if (!context.Users.Any(u => u.Role == Role.Admin))
        {
            var admin = new User
            {
                FullName = "System Admin",
                Email = "admin@mytherapy.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = Role.Admin
            };

            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}
