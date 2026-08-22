using CrispyKitchen.Application.Common.Models;
using CrispyKitchen.Application.Features.Auth.Commands.Login;
using CrispyKitchen.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrispyKitchen.WebApi.Controllers;

/// <summary>
/// Deliberately thin. Controllers should never contain business logic —
/// their only job is: receive the HTTP request, hand it to MediatR,
/// return the result. Compare this to a restaurant waiter: they take
/// your order and bring your food, they don't cook it themselves.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResult>> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}