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
using SFA.DAS.Authorization.Errors;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {
        private IAuthorisationResourceRepository _authorisationResourceRepository;
        private readonly IUserContext _userContext;

        public DefaultAuthorizationHandler(IAuthorisationResourceRepository authorisationResourceRepository,IUserContext userContext)
        {
            _authorisationResourceRepository = authorisationResourceRepository;
            _userContext = userContext;
        }

        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            if (!_userContext.IsSupportConsoleUser())
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
    }   

}