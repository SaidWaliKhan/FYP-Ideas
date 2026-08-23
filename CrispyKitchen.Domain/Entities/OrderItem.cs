using CrispyKitchen.Domain.Common;

namespace CrispyKitchen.Domain.Entities;

/// A line item within an Order. Notice it stores its OWN copy of
/// ProductName and UnitPrice, separate from the live Product record.
/// Real-world analogy: a printed receipt is frozen the moment it's
/// printed. If the shop raises the burger price next week, receipts
/// from last week don't retroactively change — and neither should
/// this order.
public class OrderItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;

    private OrderItem() { }

    public static OrderItem Create(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        return new OrderItem
        {
            ProductId = productId,
            ProductName = productName,
            UnitPrice = unitPrice,
            Quantity = quantity
        };
    }
}