using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Extensions;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmployerAccountsConfiguration _config;

    public UserContext(IHttpContextAccessor httpContextAccessor,
        EmployerAccountsConfiguration config)
    {
        _httpContextAccessor = httpContextAccessor;
        _config = config;
    }

        
    public bool IsSupportConsoleUser()
    {
        var requiredRoles = _config.SupportConsoleUsers.Split(',');
        return requiredRoles.Any(role => _httpContextAccessor.HttpContext.User.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role));
    }

    public string GetClaimValue(string key)
    {
        return _httpContextAccessor.HttpContext.User.FindFirstValue(key);
    }
}

public interface IUserContext
{
    bool IsSupportConsoleUser();
    string GetClaimValue(string key);
}