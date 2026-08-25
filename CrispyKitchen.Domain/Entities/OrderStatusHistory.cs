using CrispyKitchen.Domain.Common;
using CrispyKitchen.Domain.Enums;

namespace CrispyKitchen.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public OrderStatus PreviousStatus { get; private set; }
    public OrderStatus NewStatus { get; private set; }
    public Guid ChangedByUserId { get; private set; }
    public string ChangedByName { get; private set; } = string.Empty;

    private OrderStatusHistory() { }

    internal static OrderStatusHistory Create(OrderStatus previousStatus, OrderStatus newStatus, Guid changedByUserId, string changedByName) => new()
    {
        PreviousStatus = previousStatus,
        NewStatus = newStatus,
        ChangedByUserId = changedByUserId,
        ChangedByName = changedByName
    };
}
