using SFA.DAS.Authorization.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Results;
using System.Collections.Generic;
using SFA.DAS.Authorization.Handlers;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;
using System;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {
        private IAuthorisationResourceRepository _authorisationResourceRepository;

        public DefaultAuthorizationHandler(IAuthorisationResourceRepository authorisationResourceRepository)
        {
            _authorisationResourceRepository = authorisationResourceRepository;
        }

        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            var authorizationResult = new AuthorizationResult();
            authorizationContext.TryGet<Resource>("Resource", out var resource);
            authorizationContext.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentity);
            var resourceValue = resource != null ? resource.Value : "default";
            var userRoleClaims = claimsIdentity?.Claims.Where(c => c.Type == claimsIdentity?.RoleClaimType);
            if (userRoleClaims == null || userRoleClaims.All(claim => claim.Value != AuthorizationConstants.Tier2User))
                return Task.FromResult(authorizationResult);

            if (!IsAllowedResourceList(resourceValue)) {
                authorizationResult.AddError(new Tier2UserAccessNotGranted());
            }

            return Task.FromResult(authorizationResult);
        }     

        private bool IsAllowedResourceList(string resourceValue)
        {
            var resourceList = _authorisationResourceRepository.Get();
            return resourceList.Any(res => res.Url.Equals(resourceValue, StringComparison.InvariantCultureIgnoreCase));
        }
    }   

}