using CrispyKitchen.Domain.Enums;
using FluentValidation;

namespace CrispyKitchen.Application.Features.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Cart cannot be empty.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
        
        RuleFor(x => x.ContactPhone).NotEmpty();
        RuleFor(x => x.FulfillmentType).IsInEnum();

        When(x => x.FulfillmentType == FulfillmentType.Delivery, () =>
        {
            RuleFor(x => x.DeliveryAddress).NotEmpty().WithMessage("Delivery address is required.");
            RuleFor(x => x.DeliveryCity).NotEmpty().WithMessage("Delivery city is required.");
        });
    }
}