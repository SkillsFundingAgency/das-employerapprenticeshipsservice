using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Authentication
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IOwinWrapper _owinWrapper;

        public CurrentUserService(IOwinWrapper owinWrapper)
        {
            _owinWrapper = owinWrapper;
        }

        public CurrentUser GetCurrentUser()
        {
            var isAuthenticated = _owinWrapper.IsUserAuthenticated();

            if (!isAuthenticated)
            {
                return null;
            }

            var externalUserId = _owinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName);
            var email = _owinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);

            return new CurrentUser
            {
                ExternalUserId = externalUserId,
                Email = email
            };
        }
    }
}