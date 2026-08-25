namespace CrispyKitchen.Domain.Exceptions;

public class InsufficientStockException : Exception
{
    public InsufficientStockException(string productName, int available, int requested)
        : base($"Not enough stock for '{productName}'. Available: {available}, requested: {requested}.") { }
}