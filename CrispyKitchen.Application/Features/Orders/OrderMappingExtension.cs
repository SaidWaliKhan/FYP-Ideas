using CrispyKitchen.Domain.Entities;

namespace CrispyKitchen.Application.Features.Orders;

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order) => new(
        order.Id, 
        order.CustomerId, 
        order.Status.ToString(), 
        order.PaymentStatus.ToString(),
        order.FulfillmentType.ToString(),
        order.DeliveryAddress, 
        order.DeliveryCity, 
        order.ContactPhone,
        order.Subtotal, 
        order.DeliveryFee, 
        order.Total, 
        order.CreatedAtUtc,
        order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList(),
        order.StatusHistory.OrderBy(history => history.CreatedAtUtc).Select(history => new OrderStatusHistoryDto(history.PreviousStatus.ToString(), history.NewStatus.ToString(), history.ChangedByName, history.CreatedAtUtc)).ToList());
}
