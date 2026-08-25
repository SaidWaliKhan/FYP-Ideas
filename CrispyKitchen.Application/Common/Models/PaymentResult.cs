using CrispyKitchen.Domain.Enums;

namespace CrispyKitchen.Application.Common.Models;

public record PaymentResult(PaymentStatus Status, string ProviderReference);
