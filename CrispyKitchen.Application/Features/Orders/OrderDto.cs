namespace CrispyKitchen.Application.Features.Orders;

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
public record OrderStatusHistoryDto(string PreviousStatus, string NewStatus, string ChangedByName, DateTime ChangedAtUtc);

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    string PaymentStatus,
    string FulfillmentType,
    string? DeliveryAddress,
    string? DeliveryCity,
    string ContactPhone,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Total,
    DateTime PlacedAtUtc,
    List<OrderItemDto> Items,
    List<OrderStatusHistoryDto> StatusHistory);
