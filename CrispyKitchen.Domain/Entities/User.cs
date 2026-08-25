using CrispyKitchen.Domain.Common;
using CrispyKitchen.Domain.Enums;

namespace CrispyKitchen.Domain.Entities;

/// A registered user. Private setters + a static factory method mean
/// nobody outside this class can create a "half-built" or invalid User —
/// like a vending machine that only ever dispenses a complete product,
/// never lets you grab loose parts.
/// 
public class User : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;

    private User() { } // EF Core needs this — it builds objects without calling your constructor

    public static User Create(string fullName, string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        return new User
        {
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = role
        };
    }

    public void SetRole(UserRole role)
    {
        if (role == UserRole.Customer)
            throw new ArgumentException("Staff users cannot be assigned the Customer role.", nameof(role));

        Role = role;
        MarkUpdated();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        MarkUpdated();
    }

    public void ResetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        PasswordHash = passwordHash;
        MarkUpdated();
    }
}
