using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{request.Id}' was not found.");

        // All the state-machine logic lives in the entity itself (Step 1
        // above) — this handler just calls it. If someone tries an
        // illegal jump, Order.AdvanceTo throws and we never even get to
        // SaveChangesAsync.
        order.AdvanceTo(request.NewStatus, _currentUser.UserId, _currentUser.FullName ?? "Unknown staff member");

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order.ToDto();
    }
}
