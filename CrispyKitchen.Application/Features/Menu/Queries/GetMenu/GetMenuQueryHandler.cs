using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Queries.GetMenu;

public class GetMenuQueryHandler : IRequestHandler<GetMenuQuery, PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetMenuQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedResult<ProductDto>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _unitOfWork.Products.GetAvailablePagedAsync(
            request.PageNumber, request.PageSize, request.Search, request.Category, cancellationToken);
        return new PagedResult<ProductDto>(products.Select(product => product.ToDto()).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
