using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Queries.GetMenu;

// A Query with no parameters — it's still a "record" for consistency,
// even though it carries no data of its own.
public record GetMenuQuery(int PageNumber, int PageSize, string? Search, string? Category) : IRequest<PagedResult<ProductDto>>;
