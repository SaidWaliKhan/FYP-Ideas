using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateProductCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{request.Id}' was not found.");

        product.UpdateDetails(
            request.Name, 
            request.Description, 
            request.Price,
            request.Category, 
            request.ImageUrl, 
            request.IsFeatured);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.ToDto();
    }
}