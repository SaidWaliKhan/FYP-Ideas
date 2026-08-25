using CrispyKitchen.Domain.Common;

namespace CrispyKitchen.Domain.Entities;

public class CustomerCartItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    private CustomerCartItem() { }

    public static CustomerCartItem Create(Guid productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        return new CustomerCartItem { ProductId = productId, Quantity = quantity };
    }
}
