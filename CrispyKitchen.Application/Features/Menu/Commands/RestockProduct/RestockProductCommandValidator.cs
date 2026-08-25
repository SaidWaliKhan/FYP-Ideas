using FluentValidation;

namespace CrispyKitchen.Application.Features.Menu.Commands.RestockProduct;

public class RestockProductCommandValidator : AbstractValidator<RestockProductCommand>
{
    public RestockProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
