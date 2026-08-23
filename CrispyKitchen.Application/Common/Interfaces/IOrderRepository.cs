using CrispyKitchen.Domain.Entities;

namespace CrispyKitchen.Application.Common.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetActiveOrdersAsync(CancellationToken cancellationToken = default); // feeds the kitchen dashboard
}