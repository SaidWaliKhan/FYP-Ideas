using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Application.Common.Models;
using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.SimulatePayment;

public class SimulatePaymentCommandHandler : IRequestHandler<SimulatePaymentCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentProvider _paymentProvider;

    public SimulatePaymentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IPaymentProvider paymentProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _paymentProvider = paymentProvider;
    }

    public async Task<OrderDto> Handle(SimulatePaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{request.Id}' was not found.");

        if (order.CustomerId != _currentUser.UserId)
            throw new ForbiddenException("You are not allowed to pay for this order.");

        if (order.PaymentStatus != PaymentStatus.Pending)
            throw new ConflictException("This order already has a final payment status.");

        var result = await _paymentProvider.ProcessAsync(
            new PaymentRequest(order.Id, order.Total, "USD", request.SimulateSuccess),
            cancellationToken);

        order.SetPaymentStatus(result.Status);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order.ToDto();
    }
}
