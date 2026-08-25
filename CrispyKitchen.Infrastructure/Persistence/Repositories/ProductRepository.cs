using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using CrispyKitchen.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(List<Product> Items, int TotalCount)> GetAvailablePagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? category,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Product>().Where(product => product.IsAvailable);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(product => product.Name.Contains(search) || product.Description.Contains(search));

        if (Enum.TryParse<ProductCategory>(category, true, out var productCategory))
            query = query.Where(product => product.Category == productCategory);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(product => product.Category)
            .ThenBy(product => product.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<Product> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = Context.Set<Product>();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(product => product.Name.Contains(search) || product.Description.Contains(search));
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(product => product.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }
}
