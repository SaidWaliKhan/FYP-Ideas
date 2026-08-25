namespace CrispyKitchen.Application.Features.Cart;

public record CartItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, int StockQuantity);
public record CartDto(List<CartItemDto> Items);
