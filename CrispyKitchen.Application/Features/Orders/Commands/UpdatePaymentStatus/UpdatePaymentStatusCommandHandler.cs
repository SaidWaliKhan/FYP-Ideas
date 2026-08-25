using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.UpdatePaymentStatus;

public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<OrderDto> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{request.Id}' was not found.");

        order.SetPaymentStatus(request.PaymentStatus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order.ToDto();
    }
}
