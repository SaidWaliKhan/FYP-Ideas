using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrispyKitchen.Infrastructure.Persistence.Configurations;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.HasKey(history => history.Id);
        builder.Property(history => history.Id).ValueGeneratedNever();
        builder.Property(history => history.PreviousStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(history => history.NewStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(history => history.ChangedByName).IsRequired().HasMaxLength(100);
    }
}
