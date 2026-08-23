using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using CrispyKitchen.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    // The override we just discussed above — Items comes back populated now.
    public override async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Context.Set<Order>()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await Context.Set<Order>()
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<List<Order>> GetActiveOrdersAsync(CancellationToken cancellationToken = default)
        => await Context.Set<Order>()
            .Include(o => o.Items)
            .Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled)
            .OrderBy(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);
}