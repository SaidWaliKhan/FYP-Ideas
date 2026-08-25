using CrispyKitchen.Domain.Common;

namespace CrispyKitchen.Domain.Entities;

public class CustomerCart : BaseEntity
{
    private readonly List<CustomerCartItem> _items = new();
    public Guid CustomerId { get; private set; }
    public IReadOnlyCollection<CustomerCartItem> Items => _items.AsReadOnly();

    private CustomerCart() { }

    public static CustomerCart Create(Guid customerId, IEnumerable<CustomerCartItem> items)
    {
        var cart = new CustomerCart { CustomerId = customerId };
        cart._items.AddRange(items);
        return cart;
    }

    public void ReplaceItems(IEnumerable<CustomerCartItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
        MarkUpdated();
    }
}
