namespace CrispyKitchen.Application.Common.Models;

// The DTO (data transfer object) returned to the client after auth.
// Notice: no PasswordHash here. Never send back anything password-related.
public record AuthResult(string Token, string Email, string FullName, string Role);