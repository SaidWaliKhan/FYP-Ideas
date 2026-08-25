using CrispyKitchen.Application.Common.Models;

namespace CrispyKitchen.Application.Common.Interfaces;

public interface IPaymentProvider
{
    Task<PaymentResult> ProcessAsync(PaymentRequest request, CancellationToken cancellationToken = default);
}
