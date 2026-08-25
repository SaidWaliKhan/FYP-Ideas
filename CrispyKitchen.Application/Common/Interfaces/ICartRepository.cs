using CrispyKitchen.Domain.Entities;

namespace CrispyKitchen.Application.Common.Interfaces;

public interface ICartRepository : IRepository<CustomerCart>
{
    Task<CustomerCart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
