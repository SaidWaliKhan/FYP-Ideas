using CrispyKitchen.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence;

/// The single EF Core "gateway" to the database. Empty for now —
/// we'll add DbSet<Product>, DbSet<Order> etc. as we build each feature.


// IdentityDbContext gives us the Users/Roles/UserRoles tables for free —
// we don't hand-roll a Users table, we extend the one Identity already
// designed (and battle-tested) for us.
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {




        base.OnModelCreating(modelBuilder); // must run first — sets up Identity's own tables


        // Automatically applies every IEntityTypeConfiguration<T> class
        // in this assembly — so each entity gets its own config file
        // instead of one giant OnModelCreating method (SRP again).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}