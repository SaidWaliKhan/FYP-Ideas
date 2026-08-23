using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    // v1: flat fee. A real system would calculate this by distance/zone —
    // noted here explicitly so it's a visible simplification, not a hidden one.
    private const decimal FlatDeliveryFee = 1.50m;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public PlaceOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var orderItems = new List<OrderItem>();

        foreach (var line in request.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(line.ProductId, cancellationToken)
                ?? throw new NotFoundException($"Product with id '{line.ProductId}' was not found.");

            if (!product.IsAvailable)
                throw new ConflictException($"'{product.Name}' is currently sold out.");

            // THE critical line. Price comes from the Product record we
            // just fetched from OUR database — never from anything the
            // client sent. This single line is what makes the whole
            // "$10 burger for $1" attack impossible, structurally.
            orderItems.Add(OrderItem.Create(product.Id, product.Name, product.Price, line.Quantity));
        }

        var order = Order.Place(
            customerId: _currentUser.UserId,
            items: orderItems,
            fulfillmentType: request.FulfillmentType,
            deliveryAddress: request.DeliveryAddress,
            deliveryCity: request.DeliveryCity,
            contactPhone: request.ContactPhone,
            deliveryFee: FlatDeliveryFee);

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }
}