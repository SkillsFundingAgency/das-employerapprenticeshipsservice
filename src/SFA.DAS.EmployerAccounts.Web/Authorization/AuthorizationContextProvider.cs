using Microsoft.AspNetCore.Mvc.Infrastructure;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.EmployerFeatures.Context;
using SFA.DAS.Authorization.EmployerUserRoles.Context;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.Infrastructure;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly IHashingService _hashingService;
        private readonly IAuthenticationServiceWrapper _authenticationService;
        private readonly IActionContextAccessor _actionContextAccessor;

        public AuthorizationContextProvider(IHashingService hashingService,
            IAuthenticationServiceWrapper authenticationService,
            IActionContextAccessor actionContextAccessor)
        {
            _hashingService = hashingService;
            _authenticationService = authenticationService;
            _actionContextAccessor = actionContextAccessor;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var authorizationContext = new AuthorizationContext();
            var accountValues = GetAccountValues();
            var userValues = GetUserValues();

            if (accountValues.Id.HasValue)
            {
                authorizationContext.AddEmployerUserRoleValues(accountValues.Id.Value, userValues.Ref.Value);
            }

            authorizationContext.AddEmployerFeatureValues(accountValues.Id, userValues.Email);

            return authorizationContext;
        }

        private (string HashedId, long? Id) GetAccountValues()
        {
            if (!_actionContextAccessor.ActionContext.RouteData.Values.TryGetValue(RouteValues.EncodedAccountId, out var accountHashedId))
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