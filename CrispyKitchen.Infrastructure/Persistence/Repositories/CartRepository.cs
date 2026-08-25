using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence.Repositories;

public class CartRepository : Repository<CustomerCart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context) { }

    public Task<CustomerCart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => Context.Set<CustomerCart>().Include(cart => cart.Items).FirstOrDefaultAsync(cart => cart.CustomerId == customerId, cancellationToken);
}
