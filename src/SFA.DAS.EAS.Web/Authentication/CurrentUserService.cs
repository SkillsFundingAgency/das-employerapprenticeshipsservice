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
            var externalUserId = _owinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName);

            return externalUserId != null
                ? new CurrentUser { ExternalUserId = externalUserId }
                : null;
        }
    }
}