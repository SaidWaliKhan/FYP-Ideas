using CrispyKitchen.Domain.Enums;
using FluentValidation;

namespace CrispyKitchen.Application.Features.Auth.Commands.ManageStaff;

public class UpdateStaffRoleCommandValidator : AbstractValidator<UpdateStaffRoleCommand>
{
    public UpdateStaffRoleCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Role).Must(role => role is UserRole.Admin or UserRole.KitchenStaff);
    }
}

public class SetStaffActiveCommandValidator : AbstractValidator<SetStaffActiveCommand>
{
    public SetStaffActiveCommandValidator() => RuleFor(command => command.Id).NotEmpty();
}

public class ResetStaffPasswordCommandValidator : AbstractValidator<ResetStaffPasswordCommand>
{
    public ResetStaffPasswordCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.NewPassword).NotEmpty().MinimumLength(8).Matches("[A-Z]").Matches("[0-9]");
    }
}

public class DeleteStaffUserCommandValidator : AbstractValidator<DeleteStaffUserCommand>
{
    public DeleteStaffUserCommandValidator() => RuleFor(command => command.Id).NotEmpty();
}
