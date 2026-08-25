using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Application.Common.Models;
using CrispyKitchen.Domain.Enums;
using Microsoft.Extensions.Hosting;

namespace CrispyKitchen.Infrastructure.Payments;

/// <summary>
/// Development/testing provider only. Replace this registration with a real
/// provider implementation, such as StripePaymentProvider, in production.
/// </summary>
public class DummyPaymentProvider : IPaymentProvider
{
    private readonly IHostEnvironment _environment;

    public DummyPaymentProvider(IHostEnvironment environment) => _environment = environment;

    public Task<PaymentResult> ProcessAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        if (!_environment.IsDevelopment())
            throw new ForbiddenException("Dummy payments are available only in the Development environment.");

        var status = request.SimulateSuccess ? PaymentStatus.Paid : PaymentStatus.Failed;
        var reference = $"dummy_{status.ToString().ToLowerInvariant()}_{Guid.NewGuid():N}";
        return Task.FromResult(new PaymentResult(status, reference));
    }
}
