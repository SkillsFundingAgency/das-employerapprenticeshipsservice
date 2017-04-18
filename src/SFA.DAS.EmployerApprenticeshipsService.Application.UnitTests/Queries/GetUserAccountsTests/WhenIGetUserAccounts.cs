using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserAccountsTests
{
    class WhenIGetUserAccounts
    {
        private Mock<IUserAccountRepository> _userAccountRepository;
        private GetUserAccountsQueryHandler _getUserAccountsQueryHandler;
        private List<Domain.Data.Entities.Account.Account> _accounts;
        private Domain.Data.Entities.Account.Account _account;

        [SetUp]
        public void Arrange()
        {
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _account = new Domain.Data.Entities.Account.Account {Name = "Test", RoleId = 1};
            _accounts = new List<Domain.Data.Entities.Account.Account> {_account};
            _userAccountRepository.Setup(repository => repository.GetAccountsByUserRef("1")).ReturnsAsync(new Accounts<Domain.Data.Entities.Account.Account> { AccountList = _accounts});
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
            Assert.AreEqual("Owner",account.RoleName);
        }

    }
}
