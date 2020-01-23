using System.Configuration;
using System.Linq;
using SFA.DAS.Authentication;
using System.Security.Claims;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class AuthenticationServiceExtensions
    {
        public static bool IsSupportUser(this IAuthenticationService authenticationService)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var requiredRoles = configuration.SupportConsoleUsers.Split(',');
            return requiredRoles.Any(role => authenticationService.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role));
            //return authenticationService.HasClaim(ClaimsIdentity.DefaultRoleClaimType, Constants.Tier2User);
        }
    }
}
