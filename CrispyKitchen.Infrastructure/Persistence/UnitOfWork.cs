using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Infrastructure.Persistence.Repositories;

namespace CrispyKitchen.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IUserRepository Users { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}