using FluentValidation;

namespace CrispyKitchen.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
