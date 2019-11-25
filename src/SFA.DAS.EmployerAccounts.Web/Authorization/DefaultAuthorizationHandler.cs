using SFA.DAS.Authorization.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Results;
using System.Collections.Generic;
using SFA.DAS.Authorization.Handlers;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;
using SFA.DAS.NLog.Logger;

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
            if (userRoleClaims == null || userRoleClaims.All(claim => claim.Value != AuthorizationConstants.Tier2User))
                return Task.FromResult(authorizationResult);

            if (!CheckAllowedResourceList(resourceValue)) {
                authorizationResult.AddError(new Tier2UserAccesNotGranted());
            }

            return Task.FromResult(authorizationResult);
        }

        public bool CheckAllowedResourceList(string resourceValue)
        {
            var resourceList = ResourceList.GetListOfAllowedResources();
            return resourceList.Any(res => res.ToLower().ToString() == resourceValue.ToLower());
        }
    }

    public static class ResourceList
    {
        public static IList<string> GetListOfAllowedResources()
        {
            var resourceList = new List<string> { AuthorizationConstants.TeamViewRoute, AuthorizationConstants.TeamInvite, AuthorizationConstants.TeamReview };
            return resourceList;
        }
    }

}