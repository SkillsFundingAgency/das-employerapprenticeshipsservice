using System.Configuration;
using System.Linq;
using SFA.DAS.Authentication;
using System.Security.Claims;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class AuthenticationServiceExtensions
    {
        public static bool IsSupportUser(this IAuthenticationService authenticationService)
        {
            var requiredRoles = ConfigurationManager.AppSettings["SupportConsoleUser"].Split(',');
            return requiredRoles.Any(role => authenticationService.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role));
            //return authenticationService.HasClaim(ClaimsIdentity.DefaultRoleClaimType, Constants.Tier2User);
        }
    }
}
