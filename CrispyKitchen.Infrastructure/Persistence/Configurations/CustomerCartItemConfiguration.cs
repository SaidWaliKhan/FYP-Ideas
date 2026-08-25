using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrispyKitchen.Infrastructure.Persistence.Configurations;

public class CustomerCartItemConfiguration : IEntityTypeConfiguration<CustomerCartItem>
{
    public void Configure(EntityTypeBuilder<CustomerCartItem> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).ValueGeneratedNever();
    }
}
