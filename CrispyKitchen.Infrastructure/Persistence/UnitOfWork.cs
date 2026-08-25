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
    public ICartRepository Carts { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Products = new ProductRepository(context);
        Orders = new OrderRepository(context);
        Carts = new CartRepository(context);
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
    catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 })
    {
        throw new ConflictException("A record with the same unique value already exists.");
    }
}
}
