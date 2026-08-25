using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateProductCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name, 
            request.Description, 
            request.Price,
            request.Category, 
            request.ImageUrl, 
            request.IsFeatured,
            request.StockQuantity);

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToDto();
    }
}