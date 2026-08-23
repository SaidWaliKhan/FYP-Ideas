using CrispyKitchen.Domain.Entities;

namespace CrispyKitchen.Application.Features.Menu;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product) => new(
        product.Id, 
        product.Name, 
        product.Description, 
        product.Price,
        product.Category.ToString(), 
        product.ImageUrl,
        product.IsAvailable, 
        product.IsFeatured);
}