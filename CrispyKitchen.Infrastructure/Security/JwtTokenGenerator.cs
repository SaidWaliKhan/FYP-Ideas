using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CrispyKitchen.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> options) => _settings = options.Value;

    public string GenerateToken(User user)
    {
        // Claims = the actual information stamped onto the wristband —
        // who you are and what role you have. The server can read these
        // back out of the token on every request, without a DB lookup.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}