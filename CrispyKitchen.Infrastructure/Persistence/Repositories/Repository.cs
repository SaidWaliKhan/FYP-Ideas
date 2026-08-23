using CrispyKitchen.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence.Repositories;

public class Repository<T> : Application.Common.Interfaces.IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context;
    public Repository(ApplicationDbContext context) => Context = context;

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Context.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public  virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Context.Set<T>().ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await Context.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => Context.Set<T>().Update(entity);
    public void Remove(T entity) => Context.Set<T>().Remove(entity);
}