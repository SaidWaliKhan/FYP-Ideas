using CrispyKitchen.Domain.Entities;

namespace CrispyKitchen.Application.Common.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<(List<Order> Items, int TotalCount)> GetByCustomerPagedAsync(Guid customerId, int pageNumber, int pageSize, string? status, CancellationToken cancellationToken = default);
    Task<(List<Order> Items, int TotalCount)> GetActiveOrdersPagedAsync(int pageNumber, int pageSize, string? status, CancellationToken cancellationToken = default);
}
