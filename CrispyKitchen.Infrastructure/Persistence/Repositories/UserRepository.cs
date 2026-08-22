using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrispyKitchen.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await Context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await Context.Set<User>()
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
}