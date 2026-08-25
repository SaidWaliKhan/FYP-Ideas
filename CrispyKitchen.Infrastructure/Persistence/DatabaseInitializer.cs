using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using CrispyKitchen.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CrispyKitchen.Infrastructure.Persistence;

public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public DatabaseInitializer(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        // Apply any pending EF Core migrations.
        await _context.Database.MigrateAsync(cancellationToken);

        // Add default products only when there are no products yet.
        if (!await _context.Products.AnyAsync(cancellationToken))
        {
            var products = new[]
            {
                Product.Create(
                    "Classic Chicken Burger",
                    "Crispy chicken, lettuce, and house sauce.",
                    6.50m,
                    ProductCategory.Burgers,
                    null,
                    true,
                    25),

                Product.Create(
                    "Spicy Wings",
                    "Six crispy wings with a spicy glaze.",
                    5.00m,
                    ProductCategory.Chicken,
                    null,
                    true,
                    30),

                Product.Create(
                    "Seasoned Fries",
                    "Golden fries with house seasoning.",
                    2.50m,
                    ProductCategory.Sides,
                    null,
                    false,
                    40)
            };

            await _context.Products.AddRangeAsync(
                products,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        // If an Admin already exists, there is nothing more to do.
        var adminExists = await _context.Users.AnyAsync(
            user => user.Role == UserRole.Admin,
            cancellationToken);

        if (adminExists)
            return;

        // Read the first Admin information from configuration.
        var fullName = _configuration["BootstrapAdmin:FullName"];
        var email = _configuration["BootstrapAdmin:Email"];
        var password = _configuration["BootstrapAdmin:Password"];

        // Don't create an Admin if the required configuration is missing.
        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No administrator exists. Set BootstrapAdmin__FullName, " +
                "BootstrapAdmin__Email, and BootstrapAdmin__Password " +
                "before starting the API.");
        }

        // Hash the password before storing it.
        var passwordHash = _passwordHasher.Hash(password);

        // Create the first Admin user.
        var admin = User.Create(
            fullName,
            email,
            passwordHash,
            UserRole.Admin);

        await _context.Users.AddAsync(
            admin,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}