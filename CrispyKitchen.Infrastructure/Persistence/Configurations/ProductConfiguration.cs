using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrispyKitchen.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Description).HasMaxLength(1000);

        // Explicit precision for money: EF Core's default for decimal is
        // ambiguous and actually logs a build-time warning if you leave
        // it unset. Being explicit here is like a pharmacist writing the
        // exact dose on a label instead of "some amount" — with money,
        // vague precision silently rounds/truncates values over time.
        builder.Property(p => p.Price).HasColumnType("decimal(10,2)");

        builder.Property(p => p.Category).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
    }
}