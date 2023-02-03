using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerAccounts.Authorisation;

public interface IAuthenticationServiceWrapper
{
    string GetClaimValue(string key);
    bool IsUserAuthenticated();
    bool TryGetClaimValue(string key, out string value);
}

public class AuthenticationServiceWrapper : IAuthenticationServiceWrapper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationServiceWrapper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new NullReferenceException(nameof(httpContextAccessor));
    }

    public string GetClaimValue(string key)
    {
        return _httpContextAccessor.HttpContext.User.FindFirst(key).Value;
    }

    public bool IsUserAuthenticated()
    {
        return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
    }

    public bool TryGetClaimValue(string key, out string value)
    {
        var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
        var claim = identity?.Claims.FirstOrDefault(c => c.Type == key);

        value = claim?.Value;

        return value != null;
    }
}