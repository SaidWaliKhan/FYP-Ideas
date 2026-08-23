using MediatR;

namespace CrispyKitchen.Application.Features.Menu.Commands.SetProductAvailability;

// This IS your "delete" — soft, reversible, safe with existing orders.
public record SetProductAvailabilityCommand(Guid Id, bool IsAvailable) : IRequest<ProductDto>;