using CrispyKitchen.Domain.Common;

namespace CrispyKitchen.Application.Common.Interfaces;

// Generic contract every entity's repository follows — CRUD basics only.
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}