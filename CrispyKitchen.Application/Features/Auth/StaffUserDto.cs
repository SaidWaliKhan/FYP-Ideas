namespace CrispyKitchen.Application.Features.Auth;

public record StaffUserDto(Guid Id, string FullName, string Email, string Role, bool IsActive, DateTime CreatedAtUtc);
