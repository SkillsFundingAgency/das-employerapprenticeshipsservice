using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserAccountsTests
{
    class WhenIGetUserAccounts
    {
        private Mock<IUserAccountRepository> _userAccountRepository;
        private GetUserAccountsQueryHandler _getUserAccountsQueryHandler;
        private List<Domain.Data.Entities.Account.Account> _accounts;
        private Domain.Data.Entities.Account.Account _account;

        private readonly Guid _externalUserId = Guid.NewGuid();

        [SetUp]
        public void Arrange()
        {
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _account = new Domain.Data.Entities.Account.Account {Name = "Test", Role = Role.Owner};
            _accounts = new List<Domain.Data.Entities.Account.Account> {_account};
            _userAccountRepository.Setup(repository => repository.GetAccountsByUserRef(_externalUserId)).ReturnsAsync(new Accounts<Domain.Data.Entities.Account.Account> { AccountList = _accounts});
            _getUserAccountsQueryHandler = new GetUserAccountsQueryHandler(_userAccountRepository.Object);

        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledToGetAllUsers()
        {
            //Act
             await _getUserAccountsQueryHandler.Handle(new GetUserAccountsQuery {ExternalUserId = _externalUserId });

            //Assert
            _userAccountRepository.Verify(x => x.GetAccountsByUserRef(_externalUserId), Times.Once);
        }

        [Test]
        public async Task ThenTheRoleNameIsCorrectlMapped()
        {
            //Act
            var actual = await _getUserAccountsQueryHandler.Handle(new GetUserAccountsQuery { ExternalUserId = _externalUserId });

            //Assert
            var account = actual.Accounts.AccountList.FirstOrDefault();
            Assert.IsNotNull(account);
            Assert.AreEqual("Owner", account.Role.ToString());
        }

    }
}
