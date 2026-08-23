using CrispyKitchen.Application.Features.Orders;
using CrispyKitchen.Application.Features.Orders.Commands.PlaceOrder;
using CrispyKitchen.Application.Features.Orders.Commands.UpdateOrderStatus;
using CrispyKitchen.Application.Features.Orders.Queries.GetActiveOrders;
using CrispyKitchen.Application.Features.Orders.Queries.GetMyOrders;
using CrispyKitchen.Application.Features.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrispyKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // baseline: every action needs SOME logged-in user; roles refine it below
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<OrderDto>> PlaceOrder(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken));

    [HttpGet("mine")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<List<OrderDto>>> GetMyOrders(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetMyOrdersQuery(), cancellationToken));

    [HttpGet("active")]
    [Authorize(Roles = "Admin,KitchenStaff")]
    public async Task<ActionResult<List<OrderDto>>> GetActiveOrders(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetActiveOrdersQuery(), cancellationToken));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,KitchenStaff")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        return Ok(await _mediator.Send(command, cancellationToken));
    }
}