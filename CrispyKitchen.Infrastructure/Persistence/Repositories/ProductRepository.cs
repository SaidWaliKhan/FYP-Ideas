using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<Product>> GetAvailableAsync(CancellationToken cancellationToken = default)
        => await Context.Set<Product>()
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
}