using SFA.DAS.Authorization.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Results;
using System.Collections.Generic;
using SFA.DAS.Authorization.Handlers;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {       

        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            var authorizationResult = new AuthorizationResult();
            authorizationContext.TryGet<Resource>("Resource", out var resource);
            authorizationContext.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentity);
            var resourceValue = resource != null ? resource.Value : "default";
            var userRoleClaims = claimsIdentity?.Claims.Where(c => c.Type == claimsIdentity?.RoleClaimType);

            if (userRoleClaims != null && userRoleClaims.Any(claim => claim.Value == RouteValueKeys.Tier2User))
            {
                if (!resourceValue.ToLower().Contains(RouteValueKeys.TeamViewRoute))
                {
                    authorizationResult.AddError(new Tier2UserAccesNotGranted());
                }
            }

            return Task.FromResult(authorizationResult);
        }
    }   

}