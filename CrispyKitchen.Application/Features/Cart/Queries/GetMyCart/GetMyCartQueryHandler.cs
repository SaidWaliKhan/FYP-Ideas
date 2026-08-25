using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Cart.Queries.GetMyCart;

public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    public GetMyCartQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) => (_unitOfWork, _currentUser) = (unitOfWork, currentUser);

    public async Task<CartDto> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByCustomerIdAsync(_currentUser.UserId, cancellationToken);
        if (cart is null) return new CartDto([]);

        var items = new List<CartItemDto>();
        foreach (var item in cart.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is not null) items.Add(new CartItemDto(product.Id, product.Name, product.Price, item.Quantity, product.StockQuantity));
        }
        return new CartDto(items);
    }
}
