using CrispyKitchen.Application.Common.Models;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.Register;


// This is the "form" the client fills out. IRequest<AuthResult> tells
// MediatR: "when this command is sent, route it to whichever handler
// implements IRequestHandler<RegisterCommand, AuthResult>."
public record RegisterCommand(
    string FullName,
    string Email,
    string Password) : IRequest<AuthResult>;