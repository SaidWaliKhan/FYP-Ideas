using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    ProductCategory Category,
    string? ImageUrl,
    bool IsFeatured) : IRequest<ProductDto>;