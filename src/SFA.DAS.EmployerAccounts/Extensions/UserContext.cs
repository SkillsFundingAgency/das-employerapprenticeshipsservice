using SFA.DAS.Authentication;
using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Extensions;

public class UserContext : IUserContext
{
    private readonly IAuthenticationService _authenticationService;
    private readonly EmployerAccountsConfiguration _config;

    public UserContext(IAuthenticationService authenticationService,
        EmployerAccountsConfiguration config)
    {
        _authenticationService = authenticationService;
        _config = config;
    }

        
    public bool IsSupportConsoleUser()
    {
        var requiredRoles = _config.SupportConsoleUsers.Split(',');
        return requiredRoles.Any(role => _authenticationService.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role));
    }

    public string GetClaimValue(string key)
    {
        return _authenticationService.GetClaimValue(key);
    }
}

public interface IUserContext
{
    bool IsSupportConsoleUser();
    string GetClaimValue(string key);
}