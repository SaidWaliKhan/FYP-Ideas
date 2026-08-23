using CrispyKitchen.Application.Common.Interfaces;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Queries.GetActiveOrders;

public class GetActiveOrdersQueryHandler : IRequestHandler<GetActiveOrdersQuery, List<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetActiveOrdersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<OrderDto>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Orders.GetActiveOrdersAsync(cancellationToken);
        return orders.Select(o => o.ToDto()).ToList();
    }
}