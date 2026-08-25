using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid Id) : IRequest<OrderDto>;
