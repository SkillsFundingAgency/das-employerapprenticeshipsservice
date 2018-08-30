//using BoDi;
//using Moq;
//using SFA.DAS.Authentication;
//using SFA.DAS.EmployerFinance.Data;
//using SFA.DAS.EmployerFinance.Models.AccountTeam;
//using SFA.DAS.EmployerFinance.Models.UserProfile;
//using SFA.DAS.EmployerFinance.Web.Helpers;

//namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
//{
//    public static class UserExtensions
//    {
//        public static void SetMockAuthenticationServiceForUser(this User user, IObjectContainer objectContainer)
//        {
//            objectContainer.Resolve<Mock<IMembershipRepository>>().Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
//                .ReturnsAsync((string hashedAccountId, string externalUserId) => new MembershipView
//                {
//                    //HashedAccountId = hashedAccountId,
//                    //UserRef = externalUserId
//                });

//            //objectContainer.Resolve<Mock<IEmployerAccountRepository>>().Setup(o => o.GetClaimValue(ControllerConstants.UserRefClaimKeyName))
//            //    .Returns(user.Ref.ToString());

//            objectContainer.Resolve<Mock<IAuthenticationService>>().Setup(o => o.GetClaimValue(ControllerConstants.UserRefClaimKeyName))
//                .Returns(user.Ref.ToString());
//        }
//    }
//}