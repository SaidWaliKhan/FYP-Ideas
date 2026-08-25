namespace CrispyKitchen.Application.Common.Models;

public record PaymentRequest(Guid OrderId, decimal Amount, string Currency, bool SimulateSuccess);
