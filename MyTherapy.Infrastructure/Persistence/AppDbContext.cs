using Microsoft.EntityFrameworkCore;
using MyTherapy.Domain.Entities;

namespace MyTherapy.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<User> Users => Set<User>(); // Add DbSet properties for other entities as needed
}
