using CrispyKitchen.Domain.Entities;

namespace CrispyKitchen.Application.Common.Interfaces;

// User needs a couple of lookups the generic repo can't do (find by email).
// This is the Interface Segregation Principle — don't force every entity's
// repository to have a GetByEmailAsync method, just User's.
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}