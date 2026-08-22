namespace CrispyKitchen.Application.Common.Interfaces;

/// Abstracts "user account operations" away from Identity specifics,
/// so Application never references Microsoft.AspNetCore.Identity directly.

public interface IIdentityService
{
    Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string fullName, string email, string password, string role);

    Task<(bool Succeeded, Guid UserId, IList<string> Roles)> ValidateCredentialsAsync(
        string email, string password);
}