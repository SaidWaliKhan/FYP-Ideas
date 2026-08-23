using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    ProductCategory Category,
    string? ImageUrl,
    bool IsFeatured) : IRequest<ProductDto>;