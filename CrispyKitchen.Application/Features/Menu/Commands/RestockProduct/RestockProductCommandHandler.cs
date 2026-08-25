using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Commands.RestockProduct;

public class RestockProductCommandHandler : IRequestHandler<RestockProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public RestockProductCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ProductDto> Handle(RestockProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{request.Id}' was not found.");

        product.Restock(request.Quantity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.ToDto();
    }
}