using SFA.DAS.Authorization.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Results;
using System.Collections.Generic;
using RestSharp.Extensions;
using SFA.DAS.Authorization.Handlers;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {
        private readonly ILog _logger;

        public DefaultAuthorizationHandler(ILog logger)
        {
            _logger = logger;
        }

        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            var authorizationResult = new AuthorizationResult();
            authorizationContext.TryGet<Resource>("Resource", out var resource);
            authorizationContext.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentity);
            var resourceValue = resource != null ? resource.Value : "default";
            _logger.Info($"Requested Url : {resourceValue}");
            var userRoleClaims = claimsIdentity?.Claims.Where(c => c.Type == claimsIdentity?.RoleClaimType);
            if (userRoleClaims == null || userRoleClaims.All(claim => claim.Value != AuthorizationConstants.Tier2User))
                return Task.FromResult(authorizationResult);

            _logger.Info($"Claims Identity : {claimsIdentity}");
            if (!CheckAllowedResourceList(resourceValue)) {
                authorizationResult.AddError(new Tier2UserAccesNotGranted());
            }

            _logger.Info($"Authorization Result : {authorizationResult}");

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