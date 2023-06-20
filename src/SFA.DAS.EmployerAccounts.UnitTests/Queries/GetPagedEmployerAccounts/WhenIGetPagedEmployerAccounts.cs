using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetPagedEmployerAccounts
{
    public class WhenIGetPagedEmployerAccounts : QueryBaseTest<GetPagedAccountsQueryHandler, GetPagedEmployerAccountsQuery, GetPagedEmployerAccountsResponse>
    {
        private Mock<IEmployerAccountRepository> _employeeAccountRepository;
        public override GetPagedEmployerAccountsQuery Query { get; set; }
        public override GetPagedAccountsQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPagedEmployerAccountsQuery>> RequestValidator { get; set; }

        private Accounts<Account> _expectedAccounts;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _expectedAccounts = new Accounts<Account>
            {
                AccountList = new List<Account>(),
                AccountsCount = 1
            };

            _employeeAccountRepository = new Mock<IEmployerAccountRepository>();
            _employeeAccountRepository.Setup(x => x.GetAccounts(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_expectedAccounts);

            Query = new GetPagedEmployerAccountsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                ToDate = DateTime.MinValue.ToString()
            };       

            RequestHandler = new GetPagedAccountsQueryHandler(RequestValidator.Object, _employeeAccountRepository.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetPagedEmployerAccountsQuery>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _employeeAccountRepository.Verify(x => x.GetAccounts(Query.ToDate, Query.PageNumber, Query.PageSize), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetPagedEmployerAccountsQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            var actual = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.AreSame(_expectedAccounts.AccountList, actual.Accounts);
        }
    }
}
