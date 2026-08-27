using System.Security.Claims;
using CrispyKitchen.Application.Common.Interfaces;

namespace CrispyKitchen.WebApi.Services;

// to know about uithe current user 
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            // Internals worth knowing: ASP.NET Core's JWT bearer handler
            // silently remaps the token's short "sub" claim to the long
            // ClaimTypes.NameIdentifier URI by default when it reads the
            // token. If you go looking for a claim literally named "sub"
            // here later and can't find it, this remapping is why.
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string? Role => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

    public string? FullName => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
}
