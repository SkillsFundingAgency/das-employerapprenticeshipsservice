using System;
using System.Web;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.EmployerFeatures.Context;
using SFA.DAS.Authorization.EmployerUserRoles.Context;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerFinance.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly HttpContextBase _httpContext;
        private readonly IHashingService _hashingService;
        private readonly IAuthenticationService _authenticationService;

        public AuthorizationContextProvider(HttpContextBase httpContext, IHashingService hashingService, IAuthenticationService authenticationService)
        {
            _httpContext = httpContext;
            _hashingService = hashingService;
            _authenticationService = authenticationService;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var authorizationContext = new AuthorizationContext();
            var accountValues = GetAccountValues();
            var userValues = GetUserValues();

            if (accountValues.Id.HasValue)
            {
                authorizationContext.AddEmployerFeatureValues(accountValues.Id.Value, userValues.Email);
                authorizationContext.AddEmployerUserRoleValues(accountValues.Id.Value, userValues.Ref.Value);
            }

            return authorizationContext;
        }

        private (string HashedId, long? Id) GetAccountValues()
        {
            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
            {
                return (null, null);
            }

            if (!_hashingService.TryDecodeValue(accountHashedId.ToString(), out var accountId))
            {
                throw new UnauthorizedAccessException();
            }

            return (accountHashedId.ToString(), accountId);
        }

        private (Guid? Ref, string Email) GetUserValues()
        {
            if (!_authenticationService.IsUserAuthenticated())
            {
                return (null, null);
            }

            if (!_authenticationService.TryGetClaimValue(DasClaimTypes.Id, out var userRefClaimValue))
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userRefClaimValue, out var userRef))
            {
                throw new UnauthorizedAccessException();
            }

            if (!_authenticationService.TryGetClaimValue(DasClaimTypes.Email, out var userEmail))
            {
                throw new UnauthorizedAccessException();
            }

            return (userRef, userEmail);
        }
    }
}