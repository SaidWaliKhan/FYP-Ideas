using CrispyKitchen.Application.Features.Menu;
using CrispyKitchen.Application.Features.Menu.Commands.CreateProduct;
using CrispyKitchen.Application.Features.Menu.Commands.SetProductAvailability;
using CrispyKitchen.Application.Features.Menu.Commands.UpdateProduct;
using CrispyKitchen.Application.Features.Menu.Queries.GetMenu;
using CrispyKitchen.Application.Features.Menu.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrispyKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMediator _mediator;
    public MenuController(IMediator mediator) => _mediator = mediator;

    // Public — anyone can browse the menu without logging in.
    // Real-world equivalent: you don't need a loyalty card to read
    // the menu board, only to actually order.
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductDto>>> GetMenu(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetMenuQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetProductByIdQuery(id), cancellationToken));

    // [Authorize(Roles = "Admin")] reads the "role" claim we stamped
    // onto the JWT wristband back in the Auth step. If the token's role
    // claim isn't exactly "Admin", ASP.NET Core rejects the request
    // with a 403 before it ever reaches this method body — the
    // controller code doesn't even need an if-check for it.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        // 201 Created + Location header pointing at GetById — correct REST
        // behaviour, and free "here's where to find the thing you made."
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpPatch("{id:guid}/availability")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> SetAvailability(
        Guid id, SetProductAvailabilityCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        return Ok(await _mediator.Send(command, cancellationToken));
    }
}