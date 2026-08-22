using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;