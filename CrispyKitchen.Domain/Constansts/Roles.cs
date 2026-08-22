namespace CrispyKitchen.Domain.Constants;

/// <summary>
/// Central place for role names so we never hardcode "Admin" as a magic
/// string scattered across the codebase (typo = silent security bug).
/// </summary>
public static class Roles
{
    public const string Customer = "Customer";
    public const string Admin = "Admin";
    public const string Kitchen = "Kitchen";
}