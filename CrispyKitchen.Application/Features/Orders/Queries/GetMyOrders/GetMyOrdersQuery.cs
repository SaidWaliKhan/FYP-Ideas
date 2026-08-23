using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Queries.GetMyOrders;

public record GetMyOrdersQuery : IRequest<List<OrderDto>>;