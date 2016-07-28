using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetUserAccountsTests
{
    class WhenIGetUserAccounts
    {
        private Mock<IUserAccountRepository> _userAccountRepository;
        private GetUserAccountsQueryHandler _getUserAccountsQueryHandler;
        private List<Account> _accounts;
        private Account _account;


        [SetUp]
        public void Arrange()
        {
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _account = new Account {Name = "Test", Id = 1};
            _accounts = new List<Account> {_account};
            _userAccountRepository.Setup(repository => repository.GetAccountsByUserId("1")).ReturnsAsync(new Accounts {AccountList = _accounts});
            _getUserAccountsQueryHandler = new GetUserAccountsQueryHandler(_userAccountRepository.Object);

        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledToGetAllUsers()
        {
            //Act
             await _getUserAccountsQueryHandler.Handle(new GetUserAccountsQuery {UserId = "1"});

            //Assert
            _userAccountRepository.Verify(x => x.GetAccountsByUserId("1"), Times.Once);
        }

    }
}
