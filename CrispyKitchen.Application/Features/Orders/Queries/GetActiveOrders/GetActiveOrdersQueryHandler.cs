using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Queries.GetActiveOrders;

public class GetActiveOrdersQueryHandler : IRequestHandler<GetActiveOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetActiveOrdersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedResult<OrderDto>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _unitOfWork.Orders.GetActiveOrdersPagedAsync(request.PageNumber, request.PageSize, request.Status, cancellationToken);
        return new PagedResult<OrderDto>(orders.Select(order => order.ToDto()).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
