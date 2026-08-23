using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Commands.SetProductAvailability;

public class SetProductAvailabilityCommandHandler : IRequestHandler<SetProductAvailabilityCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetProductAvailabilityCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ProductDto> Handle(SetProductAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{request.Id}' was not found.");

        product.SetAvailability(request.IsAvailable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.ToDto();
    }
}