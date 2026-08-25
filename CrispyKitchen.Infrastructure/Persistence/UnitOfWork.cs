using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IUserRepository Users { get; }
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Products = new ProductRepository(context);
        Orders = new OrderRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    try
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateConcurrencyException)
    {
        throw new ConcurrencyConflictException(
            "One or more items were modified by another request at the same time. Please try again.");
    }
}
}