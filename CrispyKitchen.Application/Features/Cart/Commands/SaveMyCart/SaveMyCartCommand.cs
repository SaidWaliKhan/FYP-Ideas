using MediatR;

namespace CrispyKitchen.Application.Features.Cart.Commands.SaveMyCart;

public record SaveCartItemRequest(Guid ProductId, int Quantity);
public record SaveMyCartCommand(List<SaveCartItemRequest> Items) : IRequest<CartDto>;
