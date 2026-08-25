using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.ManageStaff;

public record ResetStaffPasswordCommand(Guid Id, string NewPassword) : IRequest;
