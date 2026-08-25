using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    private const decimal FlatDeliveryFee = 1.50m;
    private const int MaxConcurrencyRetries = 3;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public PlaceOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        // We retry a FEW times, not forever. A retry means "the row moved
        // under us, try again with fresh data" — a transient collision.
        // It does NOT mean "keep trying until stock magically appears."
        // InsufficientStockException below is a real business failure
        // and is deliberately never retried.
        for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
        {
            try
            {
                return await PlaceOrderAttempt(request, cancellationToken);
            }
            catch (ConcurrencyConflictException) when (attempt < MaxConcurrencyRetries)
            {
                // Someone else's order committed first — loop around and
                // re-read fresh stock levels, then try again.
            }
        }

        throw new ConflictException("High demand right now — please try placing your order again.");
    }

    private async Task<OrderDto> PlaceOrderAttempt(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var orderItems = new List<OrderItem>();

        foreach (var line in request.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(line.ProductId, cancellationToken)
                ?? throw new NotFoundException($"Product with id '{line.ProductId}' was not found.");

            if (!product.IsAvailable)
                throw new ConflictException($"'{product.Name}' is currently sold out.");

            // This throws InsufficientStockException immediately if there
            // genuinely isn't enough stock — no retry helps that, it's
            // a real "no" answer, not a timing collision.
            product.DecreaseStock(line.Quantity);

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

        // THIS is where a RowVersion collision surfaces — if another
        // request already saved a change to one of these Product rows
        // since we read it, SaveChangesAsync throws here.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }
}