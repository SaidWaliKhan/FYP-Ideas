using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetMyOrdersQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _unitOfWork.Orders.GetByCustomerPagedAsync(_currentUser.UserId, request.PageNumber, request.PageSize, request.Status, cancellationToken);
        return new PagedResult<OrderDto>(orders.Select(order => order.ToDto()).ToList(), request.PageNumber, request.PageSize, totalCount);
    }
}
