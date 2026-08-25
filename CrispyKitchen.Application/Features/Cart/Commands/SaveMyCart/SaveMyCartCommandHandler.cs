using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using MediatR;

namespace CrispyKitchen.Application.Features.Cart.Commands.SaveMyCart;

public class SaveMyCartCommandHandler : IRequestHandler<SaveMyCartCommand, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    public SaveMyCartCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) => (_unitOfWork, _currentUser) = (unitOfWork, currentUser);

    public async Task<CartDto> Handle(SaveMyCartCommand request, CancellationToken cancellationToken)
    {
        var groupedItems = request.Items.GroupBy(item => item.ProductId).Select(group => new SaveCartItemRequest(group.Key, group.Sum(item => item.Quantity))).ToList();
        var cartItems = new List<CustomerCartItem>();
        foreach (var item in groupedItems)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken) ?? throw new NotFoundException("A product in your cart was not found.");
            if (!product.IsAvailable || item.Quantity > product.StockQuantity) throw new ConflictException($"{product.Name} is no longer available in the requested quantity.");
            cartItems.Add(CustomerCartItem.Create(item.ProductId, item.Quantity));
        }

        var cart = await _unitOfWork.Carts.GetByCustomerIdAsync(_currentUser.UserId, cancellationToken);
        if (cart is null)
        {
            cart = CustomerCart.Create(_currentUser.UserId, cartItems);
            await _unitOfWork.Carts.AddAsync(cart, cancellationToken);
        }
        else cart.ReplaceItems(cartItems);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await new Queries.GetMyCart.GetMyCartQueryHandler(_unitOfWork, _currentUser).Handle(new Queries.GetMyCart.GetMyCartQuery(), cancellationToken);
    }
}
