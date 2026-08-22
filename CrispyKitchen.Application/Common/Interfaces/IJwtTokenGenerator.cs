using CrispyKitchen.Domain.Entities;

namespace CrispyKitchen.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    
    /// Builds a signed JWT containing the user's Id, email, and roles.
    
    string GenerateToken(User user);
}