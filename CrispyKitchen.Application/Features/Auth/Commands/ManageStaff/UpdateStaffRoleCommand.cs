using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.ManageStaff;

public record UpdateStaffRoleCommand(Guid Id, UserRole Role) : IRequest;
