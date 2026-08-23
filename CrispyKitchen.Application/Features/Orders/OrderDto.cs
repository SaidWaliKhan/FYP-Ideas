namespace CrispyKitchen.Application.Features.Orders;

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    string FulfillmentType,
    string? DeliveryAddress,
    string? DeliveryCity,
    string ContactPhone,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Total,
    DateTime PlacedAtUtc,
    List<OrderItemDto> Items);