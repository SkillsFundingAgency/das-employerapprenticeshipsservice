using System;
using System.Web;
using SFA.DAS.EAS.Application.Extensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Web.Authorization
{
    public class CallerContextProvider : ICallerContextProvider
    {
        private readonly HttpContextBase _httpContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IHashingService _hashingService;

        public CallerContextProvider(HttpContextBase httpContext, IAuthenticationService authenticationService, IHashingService hashingService)
        {
            _httpContext = httpContext;
            _authenticationService = authenticationService;
            _hashingService = hashingService;
        }

        public ICallerContext GetCallerContext()
        {
            var accountId = GetAccountId();
            var userExternalId = GetUserExternalId();

            return new CallerContext
            {
                AccountId = accountId,
                UserExternalId = userExternalId
            };
        }

        private long? GetAccountId()
        {
            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(ControllerConstants.AccountHashedIdRouteKeyName, out var accountHashedId))
            {
                return null;
            }

            if (!_hashingService.TryDecodeValue(accountHashedId.ToString(), out var accountId))
            {
                throw new UnauthorizedAccessException();
            }

            return accountId;
        }

        private Guid? GetUserExternalId()
        {
            if (!_authenticationService.IsUserAuthenticated())
            {
                return null;
            }

            if (!_authenticationService.TryGetClaimValue(ControllerConstants.UserExternalIdClaimKeyName, out var userExternalIdClaimValue))
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userExternalIdClaimValue, out var userExternalId))
            {
                throw new UnauthorizedAccessException();
            }

            return userExternalId;
        }
    }
}