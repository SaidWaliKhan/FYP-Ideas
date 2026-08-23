using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;