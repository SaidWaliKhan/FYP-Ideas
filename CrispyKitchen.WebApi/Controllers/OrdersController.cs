using CrispyKitchen.Application.Features.Orders;
using CrispyKitchen.Application.Features.Orders.Commands.PlaceOrder;
using CrispyKitchen.Application.Features.Orders.Commands.CancelOrder;
using CrispyKitchen.Application.Features.Orders.Commands.UpdateOrderStatus;
using CrispyKitchen.Application.Features.Orders.Commands.UpdatePaymentStatus;
using CrispyKitchen.Application.Features.Orders.Commands.SimulatePayment;
using CrispyKitchen.Application.Features.Orders.Queries.GetActiveOrders;
using CrispyKitchen.Application.Features.Orders.Queries.GetMyOrders;
using CrispyKitchen.Application.Features.Orders.Queries.GetOrderById;
using CrispyKitchen.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CrispyKitchen.WebApi.Hubs;

namespace CrispyKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // baseline: every action needs SOME logged-in user; roles refine it below
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<OrdersHub> _ordersHub;

    public OrdersController(IMediator mediator, IHubContext<OrdersHub> ordersHub)
    {
        _mediator = mediator;
        _ordersHub = ordersHub;
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<OrderDto>> PlaceOrder(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        await _ordersHub.Clients.Group(OrdersHub.KitchenGroup).SendAsync("OrderCreated", result, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken));

    [HttpGet("mine")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetMyOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null, CancellationToken cancellationToken = default)
        => Ok(await _mediator.Send(new GetMyOrdersQuery(Math.Max(pageNumber, 1), Math.Clamp(pageSize, 1, 100), status), cancellationToken));

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<OrderDto>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelOrderCommand(id), cancellationToken);
        await BroadcastOrderUpdated(result, cancellationToken);
        return Ok(result);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,KitchenStaff")]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetActiveOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, [FromQuery] string? status = null, CancellationToken cancellationToken = default)
        => Ok(await _mediator.Send(new GetActiveOrdersQuery(Math.Max(pageNumber, 1), Math.Clamp(pageSize, 1, 100), status), cancellationToken));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,KitchenStaff")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        var result = await _mediator.Send(command, cancellationToken);
        await BroadcastOrderUpdated(result, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/payment-status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> UpdatePaymentStatus(Guid id, UpdatePaymentStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        var result = await _mediator.Send(command, cancellationToken);
        await _ordersHub.Clients.Group(OrdersHub.OrderGroup(result.Id)).SendAsync("OrderUpdated", result, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/payment/simulate")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<OrderDto>> SimulatePayment(Guid id, SimulatePaymentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        var result = await _mediator.Send(command, cancellationToken);
        await _ordersHub.Clients.Group(OrdersHub.OrderGroup(result.Id)).SendAsync("OrderUpdated", result, cancellationToken);
        return Ok(result);
    }

    private async Task BroadcastOrderUpdated(OrderDto order, CancellationToken cancellationToken)
    {
        await _ordersHub.Clients.Group(OrdersHub.OrderGroup(order.Id)).SendAsync("OrderUpdated", order, cancellationToken);
        await _ordersHub.Clients.Group(OrdersHub.KitchenGroup).SendAsync("OrderUpdated", order, cancellationToken);
    }
}
