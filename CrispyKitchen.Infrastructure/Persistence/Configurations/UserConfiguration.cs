using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrispyKitchen.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // Same bug you already hit in SMS: since BaseEntity generates the
        // Guid in C# (not the database), EF must be told NOT to also try
        // generating one — otherwise you get that DbUpdateConcurrencyException
        // again on insert.
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique(); // enforced at DB level too, not just app level
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(50);
    }
}