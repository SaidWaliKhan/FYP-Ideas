using CrispyKitchen.Application.Features.Auth.Commands.Register;
using FluentValidation;

namespace CrispyKitchen.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
    }
}