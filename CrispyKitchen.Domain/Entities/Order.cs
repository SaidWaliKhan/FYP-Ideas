using CrispyKitchen.Domain.Common;
using CrispyKitchen.Domain.Enums;
using CrispyKitchen.Domain.Exceptions;

namespace CrispyKitchen.Domain.Entities;

/// <summary>
/// The aggregate root. "Aggregate root" just means: Order is the ONLY
/// entry point for changing anything about this order — you can't reach
/// into an OrderItem and edit it directly from outside. Real-world
/// analogy: you don't renegotiate one item on a restaurant receipt with
/// the cashier directly; every change goes through the order as a whole.
/// </summary>
public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public FulfillmentType FulfillmentType { get; private set; }

    public string? DeliveryAddress { get; private set; }
    public string? DeliveryCity { get; private set; }
    public string ContactPhone { get; private set; } = string.Empty;

    public decimal Subtotal => _items.Sum(i => i.LineTotal); // computed, never stored
    public decimal DeliveryFee { get; private set; }
    public decimal Total => Subtotal + DeliveryFee;           // computed, never stored

    private Order() { }

    public static Order Place(
        Guid customerId,
        List<OrderItem> items,
        FulfillmentType fulfillmentType,
        string? deliveryAddress,
        string? deliveryCity,
        string contactPhone,
        decimal deliveryFee)
    {
        if (items.Count == 0)
            throw new ArgumentException("An order must contain at least one item.", nameof(items));

        if (fulfillmentType == FulfillmentType.Delivery &&
            (string.IsNullOrWhiteSpace(deliveryAddress) || string.IsNullOrWhiteSpace(deliveryCity)))
            throw new ArgumentException("Delivery address and city are required for delivery orders.");

        var order = new Order
        {
            CustomerId = customerId,
            FulfillmentType = fulfillmentType,
            DeliveryAddress = deliveryAddress,
            DeliveryCity = deliveryCity,
            ContactPhone = contactPhone,
            DeliveryFee = fulfillmentType == FulfillmentType.Delivery ? deliveryFee : 0,
            Status = OrderStatus.Pending
        };

        order._items.AddRange(items);
        return order;
    }

    // THE state machine. A map of "from status -> allowed next statuses."
    // Anything not listed here is automatically illegal — you don't have
    // to remember to guard against every bad transition individually,
    // the dictionary itself IS the rulebook.
    private static readonly Dictionary<OrderStatus, OrderStatus[]> ValidTransitions = new()
    {
        [OrderStatus.Pending] = new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
        [OrderStatus.Confirmed] = new[] { OrderStatus.Preparing, OrderStatus.Cancelled },
        [OrderStatus.Preparing] = new[] { OrderStatus.Ready },
        [OrderStatus.Ready] = new[] { OrderStatus.OutForDelivery, OrderStatus.Delivered }, // Delivered directly covers Pickup
        [OrderStatus.OutForDelivery] = new[] { OrderStatus.Delivered },
        [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
        [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
    };

    public void AdvanceTo(OrderStatus newStatus)
    {
        if (!ValidTransitions.TryGetValue(Status, out var allowed) || !allowed.Contains(newStatus))
            throw new InvalidOrderTransitionException(Status, newStatus);

        Status = newStatus;
        MarkUpdated();
    }
}