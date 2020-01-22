using SFA.DAS.Authorization.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Results;
using System.Collections.Generic;
using SFA.DAS.Authorization.Handlers;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;
using System;
using System.Web;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.Authorization.Errors;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {
        private readonly HttpContextBase _httpContext;
        private IAuthorisationResourceRepository _authorisationResourceRepository;

        public DefaultAuthorizationHandler(IAuthorisationResourceRepository authorisationResourceRepository, HttpContextBase httpContext)
        {
            _authorisationResourceRepository = authorisationResourceRepository;
            _httpContext = httpContext;
        }

        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {

            //if (!IsTier2User(authorizationContext)){
            //    return IsAuthorizedResult();
            //}

            if (!Helpers.AccountTaskHelper.IsSupportConsoleUser(_httpContext.User))
            {
                return IsAuthorizedResult();
            }

            if (!IsAuthorized(GetResource(authorizationContext), authorizationContext)) {
                return IsNotAuthorizedResult(new Tier2UserAccessNotGranted());
            }

            return IsAuthorizedResult();
        }     


        private Task<AuthorizationResult> IsAuthorizedResult()
        {
            return Task.FromResult(new AuthorizationResult());
        }

        private Task<AuthorizationResult> IsNotAuthorizedResult(AuthorizationError authorizationError)
        {
            return Task.FromResult(new AuthorizationResult().AddError(authorizationError));
        }

        private bool IsAuthorized(string resourceValue, IAuthorizationContext authorizationContext)
        {
            authorizationContext.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentity);
            var resourceList = _authorisationResourceRepository.Get(claimsIdentity);
            return resourceList.Any(res => res.Value.Equals(resourceValue, StringComparison.OrdinalIgnoreCase));
        }

        private string GetResource(IAuthorizationContext authorizationContext)
        {
            authorizationContext.TryGet<Resource>("Resource", out var resource);
            return resource != null ? resource.Value : "default";
        }

        //private bool IsTier2User(IAuthorizationContext authorizationContext)
        //{
        //    authorizationContext.TryGet<ClaimsIdentity>("ClaimsIdentity", out var claimsIdentity);
        //    var userRoleClaims = claimsIdentity?.Claims.Where(c => c.Type == claimsIdentity.RoleClaimType);
        //    //if (userRoleClaims != null)
        //    //{
        //    //  foreach (var requiredRole in requiredRoles)
        //    //    {
        //    //        if (userRoleClaims.Any(claim => claim.Value.Equals(requiredRole, StringComparison.OrdinalIgnoreCase)))
        //    //        {
        //    //            return true;
        //    //        }
        //    //    }
        //    //}
        //    if (userRoleClaims != null && userRoleClaims
        //            .Any(claim => claim.Value.Equals(AuthorizationConstants.Tier2User, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

    }   

}