using System.Linq;
using SFA.DAS.Authentication;
using System.Security.Claims;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class AuthenticationServiceExtensions
    {
        public static bool IsSupportConsoleUser(this IAuthenticationService authenticationService,
            string configSupportConsoleUsers)
        {
            var requiredRoles = configSupportConsoleUsers.Split(',');
            return requiredRoles.Any(role => authenticationService.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role));
        }
    }
}
