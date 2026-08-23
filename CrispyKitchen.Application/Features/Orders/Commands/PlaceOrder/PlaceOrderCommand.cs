using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Commands.PlaceOrder;

// Just ProductId + Quantity. No price field exists here — a client
// cannot send a price even if it wanted to, there's nowhere to put it.
public record PlaceOrderItemRequest(Guid ProductId, int Quantity);

public record PlaceOrderCommand(
    List<PlaceOrderItemRequest> Items,
    FulfillmentType FulfillmentType,
    string? DeliveryAddress,
    string? DeliveryCity,
    string ContactPhone) : IRequest<OrderDto>;