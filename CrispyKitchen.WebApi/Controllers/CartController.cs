using CrispyKitchen.Application.Features.Cart;
using CrispyKitchen.Application.Features.Cart.Commands.SaveMyCart;
using CrispyKitchen.Application.Features.Cart.Queries.GetMyCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrispyKitchen.WebApi.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator) => _mediator = mediator;
    [HttpGet] public async Task<ActionResult<CartDto>> Get(CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetMyCartQuery(), cancellationToken));
    [HttpPut] public async Task<ActionResult<CartDto>> Save(SaveMyCartCommand command, CancellationToken cancellationToken) => Ok(await _mediator.Send(command, cancellationToken));
}
