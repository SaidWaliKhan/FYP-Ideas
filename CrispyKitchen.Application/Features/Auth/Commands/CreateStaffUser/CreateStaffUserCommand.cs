using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.CreateStaffUser;

public record CreateStaffUserCommand(
    string FullName,
    string Email,
    string Password,
    UserRole Role) : IRequest;
