using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence;

/// The single EF Core "gateway" to the database. Empty for now —
/// we'll add DbSet<Product>, DbSet<Order> etc. as we build each feature.

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically applies every IEntityTypeConfiguration<T> class
        // in this assembly — so each entity gets its own config file
        // instead of one giant OnModelCreating method (SRP again).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}