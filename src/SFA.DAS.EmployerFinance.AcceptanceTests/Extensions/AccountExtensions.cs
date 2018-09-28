using BoDi;
using Moq;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.AccountTeam;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class AccountExtensions
    {

        public static Account SetupAuthorizedUser(this Account account, IObjectContainer objectContainer)
        {
            objectContainer.Resolve<Mock<IMembershipRepository>>().Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string hashedAccountId, string externalUserId) => new MembershipView
                {
                    //HashedAccountId = hashedAccountId,
                    //UserRef = externalUserId
                });

            objectContainer.Resolve<Mock<IEmployerAccountRepository>>()
                .Setup(x => x.GetAccountByHashedId(It.IsAny<string>()))
                .ReturnsAsync(account);

            return account;
        }
    }
}
