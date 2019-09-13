using System;
using BoDi;
using Moq;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class AccountExtensions
    {

        public static Account SetupAuthorizedUser(this Account account, IObjectContainer objectContainer)
        {
            throw new NotImplementedException();

//            objectContainer.Resolve<Mock<IMembershipRepository>>().Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
//                .ReturnsAsync((string hashedAccountId, string externalUserId) => new MembershipView());

            objectContainer.Resolve<Mock<IEmployerAccountRepository>>()
                .Setup(x => x.GetAccountById(It.IsAny<long>()))
                .ReturnsAsync(account);

            return account;
        }
    }
}
