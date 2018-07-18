using Moq;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.TestCommon.Extensions
{
    public static class UserExtensions
    {
        public static void SetMockAuthenticationServiceForUser(this User user, Mock<IAuthenticationService> authServiceMock)
        {
            authServiceMock.Setup(o => o.GetClaimValue(ControllerConstants.UserRefClaimKeyName))
                .Returns(user.Ref.ToString());
        }
    }
}