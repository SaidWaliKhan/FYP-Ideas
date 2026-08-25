using FluentValidation;

namespace CrispyKitchen.Application.Features.Cart.Commands.SaveMyCart;

public class SaveMyCartCommandValidator : AbstractValidator<SaveMyCartCommand>
{
    public SaveMyCartCommandValidator() => RuleForEach(command => command.Items).ChildRules(item => { item.RuleFor(x => x.ProductId).NotEmpty(); item.RuleFor(x => x.Quantity).GreaterThan(0); });
}
