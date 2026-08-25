using FluentValidation;

namespace CrispyKitchen.Application.Features.Auth.Commands.RecoverCustomerPassword;

public class RecoverCustomerPasswordCommandValidator : AbstractValidator<RecoverCustomerPasswordCommand>
{
    public RecoverCustomerPasswordCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
    }
}
