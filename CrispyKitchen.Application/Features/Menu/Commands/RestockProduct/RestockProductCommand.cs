using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Commands.RestockProduct;

public record RestockProductCommand(Guid Id, int Quantity) : IRequest<ProductDto>;