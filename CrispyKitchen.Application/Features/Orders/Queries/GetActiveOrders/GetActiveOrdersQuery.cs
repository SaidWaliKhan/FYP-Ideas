using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Queries.GetActiveOrders;

public record GetActiveOrdersQuery : IRequest<List<OrderDto>>;