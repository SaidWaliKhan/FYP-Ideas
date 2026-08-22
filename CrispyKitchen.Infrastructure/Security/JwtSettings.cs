namespace CrispyKitchen.Infrastructure.Security;

// A plain settings class that mirrors the "Jwt" section of appsettings.json.
// Binding config to a strongly-typed class beats scattering
// configuration["Jwt:Secret"] magic strings all over the codebase.
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}