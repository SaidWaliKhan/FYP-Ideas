using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrispyKitchen.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();
        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(150);
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");
        builder.Ignore(i => i.LineTotal); // computed, not stored — same reasoning as Order.Total
    }
}