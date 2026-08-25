using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.UpdatePaymentStatus;

public record UpdatePaymentStatusCommand(Guid Id, PaymentStatus PaymentStatus) : IRequest<OrderDto>;
