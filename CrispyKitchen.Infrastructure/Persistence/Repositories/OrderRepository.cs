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
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<(List<Order> Items, int TotalCount)> GetByCustomerPagedAsync(Guid customerId, int pageNumber, int pageSize, string? status, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = Context.Set<Order>()
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .Where(o => o.CustomerId == customerId);

        if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            query = query.Where(order => order.Status == orderStatus);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<(List<Order> Items, int TotalCount)> GetActiveOrdersPagedAsync(int pageNumber, int pageSize, string? status, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = Context.Set<Order>()
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled);

        if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            query = query.Where(order => order.Status == orderStatus);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(o => o.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }
}
