using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid Id, OrderStatus NewStatus) : IRequest<OrderDto>;