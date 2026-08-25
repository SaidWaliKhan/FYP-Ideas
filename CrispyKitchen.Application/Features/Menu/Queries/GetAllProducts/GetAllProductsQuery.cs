using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Queries.GetAllProducts;

public record GetAllProductsQuery(int PageNumber, int PageSize, string? Search) : IRequest<PagedResult<ProductDto>>;
