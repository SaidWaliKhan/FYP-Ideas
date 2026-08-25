using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _unitOfWork.Products.GetAllPagedAsync(request.PageNumber, request.PageSize, request.Search, cancellationToken);
        return new PagedResult<ProductDto>(products.Select(product => product.ToDto()).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
