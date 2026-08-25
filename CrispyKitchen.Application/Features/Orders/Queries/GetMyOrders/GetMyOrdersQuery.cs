using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Queries.GetMyOrders;

public record GetMyOrdersQuery(int PageNumber, int PageSize, string? Status) : IRequest<PagedResult<OrderDto>>;
