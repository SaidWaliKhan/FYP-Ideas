using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.SimulatePayment;

public record SimulatePaymentCommand(Guid Id, bool SimulateSuccess) : IRequest<OrderDto>;
