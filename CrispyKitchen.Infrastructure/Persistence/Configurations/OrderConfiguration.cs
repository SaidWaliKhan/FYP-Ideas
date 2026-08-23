using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrispyKitchen.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(o => o.FulfillmentType).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.DeliveryAddress).HasMaxLength(300);
        builder.Property(o => o.DeliveryCity).HasMaxLength(100);
        builder.Property(o => o.ContactPhone).HasMaxLength(30);
        builder.Property(o => o.DeliveryFee).HasColumnType("decimal(10,2)");

        // Subtotal and Total are C# get-only properties calculated from
        // Items — they don't exist as real database columns, so we tell
        // EF explicitly to ignore them rather than error out trying to map them.
        builder.Ignore(o => o.Subtotal);
        builder.Ignore(o => o.Total);

        // Items is exposed as IReadOnlyCollection<OrderItem> backed by a
        // private List<OrderItem> field — there's no public setter for
        // EF to use. This line tells EF: populate the field directly,
        // don't look for a property setter that doesn't exist.
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}