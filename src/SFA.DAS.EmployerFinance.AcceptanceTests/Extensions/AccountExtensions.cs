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
            objectContainer.Resolve<Mock<IEmployerAccountRepository>>()
                .Setup(x => x.Get(It.IsAny<long>()))
                .ReturnsAsync(account);

            return account;
        }
    }
}
