using System;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Authentication
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IOwinWrapper _owinService;

        public CurrentUserService(IOwinWrapper owinService)
        {
            _owinService = owinService;
        }

        public CurrentUser GetCurrentUser()
        {
            var isAuthenticated = _owinService.IsUserAuthenticated();

            if (!isAuthenticated)
            {
                return null;
            }

            var externalId = Guid.Parse(_owinService.GetClaimValue(ControllerConstants.SubClaimKeyName));
            var email = _owinService.GetClaimValue(ControllerConstants.EmailClaimKeyName);

            return new CurrentUser
            {
                ExternalId = externalId,
                Email = email
            };
        }
    }
}