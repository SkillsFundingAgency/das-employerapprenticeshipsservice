using SFA.DAS.Authorization.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Results;
using System.Collections.Generic;
using SFA.DAS.Authorization.Handlers;
using static SFA.DAS.EmployerAccounts.Web.Authorization.ImpersonationAuthorizationContext;
using System;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.Authorization.Errors;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {
        private IAuthorisationResourceRepository _authorisationResourceRepository;
        private readonly EmployerAccountsConfiguration _config;
        private readonly IAuthenticationService _authenticationService;

        public DefaultAuthorizationHandler(IAuthorisationResourceRepository authorisationResourceRepository, EmployerAccountsConfiguration config, IAuthenticationService authenticationService)
        {
            _authorisationResourceRepository = authorisationResourceRepository;
            _config = config;
            _authenticationService = authenticationService;
        }

        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            if (!_authenticationService.IsSupportConsoleUser(_config.SupportConsoleUsers))
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