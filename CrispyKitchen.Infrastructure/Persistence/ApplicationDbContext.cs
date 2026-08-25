using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence;

/// The single EF Core gateway to the database.
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<CustomerCart> Carts => Set<CustomerCart>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {




        base.OnModelCreating(modelBuilder); // must run first — sets up Identity's own tables


        // Automatically applies every IEntityTypeConfiguration<T> class
        // in this assembly — so each entity gets its own config file
        // instead of one giant OnModelCreating method (SRP again).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
