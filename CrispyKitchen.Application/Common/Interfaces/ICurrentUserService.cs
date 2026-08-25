namespace CrispyKitchen.Application.Common.Interfaces;

/// <summary>
/// Real-world analogy: think of this as the name badge a security guard
/// already checked before you got to your desk. By the time your request
/// reaches a handler, "who is this person" has already been resolved from
/// the JWT — handlers just ask this service instead of parsing tokens
/// themselves everywhere.
/// </summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Role { get; }
    string? FullName { get; }
}
