using CrispyKitchen.Application.Features.Auth.Commands.CreateStaffUser;
using CrispyKitchen.Application.Features.Auth.Commands.ManageStaff;
using CrispyKitchen.Application.Features.Auth.Queries.GetStaffUsers;
using CrispyKitchen.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrispyKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("staff")]
    public async Task<ActionResult<List<StaffUserDto>>> GetStaff(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetStaffUsersQuery(), cancellationToken));

    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff(CreateStaffUserCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("staff/{id:guid}/role")]
    public async Task<IActionResult> UpdateRole(Guid id, UpdateStaffRoleCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("staff/{id:guid}/active")]
    public async Task<IActionResult> SetActive(Guid id, SetStaffActiveCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("staff/{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id, ResetStaffPasswordCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("staff/{id:guid}")]
    public async Task<IActionResult> DeleteStaff(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteStaffUserCommand(id), cancellationToken);
        return NoContent();
    }
}
