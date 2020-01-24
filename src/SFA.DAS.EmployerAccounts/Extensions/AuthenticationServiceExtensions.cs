using SFA.DAS.Authentication;
using System.Security.Claims;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class AuthenticationServiceExtensions
    {
        public static bool IsSupportUser(this IAuthenticationService authenticationService)
        {   
            return authenticationService.HasClaim(ClaimsIdentity.DefaultRoleClaimType, Constants.Tier2User);
        }
    }
}
