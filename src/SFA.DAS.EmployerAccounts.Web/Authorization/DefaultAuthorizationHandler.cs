using SFA.DAS.Authorization.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.Results;
using System.Collections.Generic;
using SFA.DAS.Authorization.Errors;
using System.Web.Routing;


namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {
        private const string Tier2User = "Tier2User";

        public Task<AuthorizationResult> GetAuthorizationResultDefault(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            var authorizationResult = new AuthorizationResult();
            authorizationContext.TryGet<RouteData>("RouteData", out var routeData);
            authorizationContext.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentity);
            var link = routeData?.Route as Route;
            var userRoleClaims = claimsIdentity?.Claims.Where(c => c.Type == claimsIdentity?.RoleClaimType);

            if (userRoleClaims != null && userRoleClaims.Any(claim => claim.Value == Tier2User))
            {
                if (!link.Url.ToString().ToLower().Contains("accounts/{hashedaccountid}/teams/view"))
                {
                    authorizationResult.AddError(new Tier2UserAccesNotGranted());
                }
            }

            return Task.FromResult(authorizationResult);
        }
    }

    public class Tier2UserAccesNotGranted : AuthorizationError
    {
        public Tier2UserAccesNotGranted() : base("Tier2 User permission is not granted")
        {

        }
    }
    
}