using BoDi;
using Moq;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions
{
    public static class UserExtensions
    {
        public static void SetMockAuthenticationServiceForUser(this User user, IObjectContainer objectContainer)
        {
            objectContainer.Resolve<Mock<IAuthenticationService>>().Setup(o => o.GetClaimValue(ControllerConstants.UserRefClaimKeyName))
                .Returns(user.Ref.ToString());
        }
    }
}