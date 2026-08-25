using FluentValidation;

namespace CrispyKitchen.Application.Features.Orders.Commands.SimulatePayment;

public class SimulatePaymentCommandValidator : AbstractValidator<SimulatePaymentCommand>
{
    public SimulatePaymentCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
