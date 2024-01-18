using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Web.Api.Extensions.CurrentUserService;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly List<Claim> _claimsIdentity;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var identity = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
        _claimsIdentity = identity!.Claims.ToList();
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated
        => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated
           ?? false;

    public Guid UserId
    {
        get
        {
            var value = _claimsIdentity[0].Value;
            return Guid.Parse(value);
        }
    }

    public string UserEmail{
        get
        {
            var value = _claimsIdentity[1].Value;
            return value ?? "";
        }
    }
}