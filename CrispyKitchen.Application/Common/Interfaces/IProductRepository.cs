using CrispyKitchen.Domain.Entities;

namespace CrispyKitchen.Application.Common.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    // Customers should only ever see available items — kitchen-sold-out
    // items shouldn't even appear in the menu response, not just be
    // greyed out. Filtering happens here, once, not in every caller.
    Task<(List<Product> Items, int TotalCount)> GetAvailablePagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? category,
        CancellationToken cancellationToken = default);
    Task<(List<Product> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default);
}
