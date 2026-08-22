namespace CrispyKitchen.Application.Common.Interfaces;

// Real-world analogy: a restaurant receipt. Multiple items get added to
// the order throughout the meal, but nothing actually happens until the
// receipt is closed out — one save, one atomic transaction, not a
// separate database trip per change.
public interface IUnitOfWork
{
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}