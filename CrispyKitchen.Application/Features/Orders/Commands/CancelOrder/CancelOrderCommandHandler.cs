using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CancelOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{request.Id}' was not found.");

        if (order.CustomerId != _currentUser.UserId)
            throw new ForbiddenException("You are not allowed to cancel this order.");

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
            throw new ConflictException("Only pending or confirmed orders can be cancelled.");

        order.AdvanceTo(OrderStatus.Cancelled, _currentUser.UserId, _currentUser.FullName ?? "Customer");

        foreach (var item in order.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken);
            product?.Restock(item.Quantity);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order.ToDto();
    }
}
