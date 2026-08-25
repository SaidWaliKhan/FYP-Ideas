using FluentValidation;

namespace CrispyKitchen.Application.Features.Orders.Commands.UpdatePaymentStatus;

public class UpdatePaymentStatusCommandValidator : AbstractValidator<UpdatePaymentStatusCommand>
{
    public UpdatePaymentStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PaymentStatus).IsInEnum();
    }
}
