using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserAccountsTests
{
    class WhenIGetUserAccounts
    {
        private Mock<IUserAccountRepository> _userAccountRepository;
        private GetUserAccountsQueryHandler _getUserAccountsQueryHandler;
        private List<Domain.Models.Account.Account> _accounts;
        private Domain.Models.Account.Account _account;

        [SetUp]
        public void Arrange()
        {
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _account = new Domain.Models.Account.Account {Name = "Test", Role = Role.Owner};
            _accounts = new List<Domain.Models.Account.Account> {_account};
            _userAccountRepository.Setup(repository => repository.GetAccountsByUserRef("1")).ReturnsAsync(new Accounts<Domain.Models.Account.Account> { AccountList = _accounts});
            _getUserAccountsQueryHandler = new GetUserAccountsQueryHandler(_userAccountRepository.Object);

        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledToGetAllUsers()
        {
            //Act
             await _getUserAccountsQueryHandler.Handle(new GetUserAccountsQuery {UserRef = "1"});

            //Assert
            _userAccountRepository.Verify(x => x.GetAccountsByUserRef("1"), Times.Once);
        }

        [Test]
        public async Task ThenTheRoleNameIsCorrectlMapped()
        {
            //Act
            var actual = await _getUserAccountsQueryHandler.Handle(new GetUserAccountsQuery { UserRef = "1" });

            //Assert
            var account = actual.Accounts.AccountList.FirstOrDefault();
            Assert.IsNotNull(account);
            Assert.AreEqual("Owner", account.RoleName);
        }

    }
}
