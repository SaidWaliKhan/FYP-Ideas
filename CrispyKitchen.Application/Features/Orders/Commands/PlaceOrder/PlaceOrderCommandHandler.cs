using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CrispyKitchen.Application.Features.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    private const decimal FlatDeliveryFee = 1.50m;
    private const int MaxConcurrencyRetries = 3;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<PlaceOrderCommandHandler> _logger;


    public PlaceOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, ILogger<PlaceOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
         _logger = logger;


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
                var result = await PlaceOrderAttempt(request, cancellationToken);

                // {OrderId} and {CustomerId} are STRUCTURED placeholders,
                // not string interpolation — Serilog stores each as its
                // own searchable field on the log event, not baked into
                // a flat sentence. That's the entire point: later you can
                // filter logs by OrderId directly instead of regex-hunting
                // through text.
                _logger.LogInformation(
                    "Order {OrderId} placed by customer {CustomerId} for {Total:C}",
                    result.Id, _currentUser.UserId, result.Total);

                return result;
            }
            catch (ConcurrencyConflictException) when (attempt < MaxConcurrencyRetries)
            {
                _logger.LogWarning(
                    "Concurrency conflict placing order for customer {CustomerId}, attempt {Attempt}",
                    _currentUser.UserId, attempt);
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