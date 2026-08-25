using MediatR;

namespace CrispyKitchen.Application.Features.Cart.Queries.GetMyCart;

public record GetMyCartQuery : IRequest<CartDto>;
