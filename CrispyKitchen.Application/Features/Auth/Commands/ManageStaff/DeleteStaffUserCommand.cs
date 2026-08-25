using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.ManageStaff;

public record DeleteStaffUserCommand(Guid Id) : IRequest;
