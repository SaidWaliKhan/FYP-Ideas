using CrispyKitchen.Domain.Enums;
using FluentValidation;

namespace CrispyKitchen.Application.Features.Auth.Commands.CreateStaffUser;

public class CreateStaffUserCommandValidator : AbstractValidator<CreateStaffUserCommand>
{
    public CreateStaffUserCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        RuleFor(x => x.Role).Must(role => role is UserRole.Admin or UserRole.KitchenStaff)
            .WithMessage("Role must be Admin or KitchenStaff.");
    }
}
