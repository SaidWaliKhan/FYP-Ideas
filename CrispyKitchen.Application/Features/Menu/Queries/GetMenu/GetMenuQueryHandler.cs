using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Queries.GetMenu;

public class GetMenuQueryHandler : IRequestHandler<GetMenuQuery, List<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetMenuQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<ProductDto>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetAvailableAsync(cancellationToken);
        return products.Select(p => p.ToDto()).ToList();
    }
}