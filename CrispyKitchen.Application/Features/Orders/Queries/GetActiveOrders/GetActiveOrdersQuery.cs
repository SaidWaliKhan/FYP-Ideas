using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Orders.Queries.GetActiveOrders;

public record GetActiveOrdersQuery(int PageNumber, int PageSize, string? Status) : IRequest<PagedResult<OrderDto>>;
