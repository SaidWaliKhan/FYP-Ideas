using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{request.Id}' was not found.");

        // This is more than the [Authorize(Roles=...)] attribute can do —
        // that only checks WHICH role you are. This checks whether YOU,
        // specifically, own THIS specific order. A Customer role alone
        // doesn't mean you can see everyone's orders, only your own.
        var isOwner = order.CustomerId == _currentUser.UserId;
        var isStaff = _currentUser.Role is "Admin" or "KitchenStaff";

        if (!isOwner && !isStaff)
            throw new ForbiddenException("You are not allowed to view this order.");

        return order.ToDto();
    }
}