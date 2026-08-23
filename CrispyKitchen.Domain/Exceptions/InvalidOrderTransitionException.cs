using CrispyKitchen.Domain.Enums;

namespace CrispyKitchen.Domain.Exceptions;

public class InvalidOrderTransitionException : Exception
{
    public InvalidOrderTransitionException(OrderStatus from, OrderStatus to)
        : base($"Cannot move an order from '{from}' to '{to}'.") { }
}