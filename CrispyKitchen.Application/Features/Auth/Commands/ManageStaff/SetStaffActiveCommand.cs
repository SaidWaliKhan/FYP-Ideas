using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.ManageStaff;

public record SetStaffActiveCommand(Guid Id, bool IsActive) : IRequest;
