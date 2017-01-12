using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPagedEmployerAccountsByDateRangeTests
{
    public class WhenIGetPagedEmployerAccounts : QueryBaseTest<GetPagedEmployerAccountsByDateRangeHandler, GetPagedEmployerAccountsByDateRangeQuery,GetPagedEmployerAccountsByDateRangeResponse>
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        public override GetPagedEmployerAccountsByDateRangeQuery Query { get; set; }
        public override GetPagedEmployerAccountsByDateRangeHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPagedEmployerAccountsByDateRangeQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _employerAccountRepository.Setup(
                x => x.GetAccountsByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(),
                        It.IsAny<int>())).ReturnsAsync(new Accounts<AccountInformation>
                        {
                            AccountList = new List<AccountInformation> {new AccountInformation()},
                            AccountsCount = 1
                        });

            Query = new GetPagedEmployerAccountsByDateRangeQuery
            {
                FromDate = DateTime.MinValue,
                ToDate = DateTime.MaxValue,
                PageNumber = 10,
                PageSize = 1000
            };

            RequestHandler = new GetPagedEmployerAccountsByDateRangeHandler(RequestValidator.Object,_employerAccountRepository.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetPagedEmployerAccountsByDateRangeQuery>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAccountRepository.Verify(x=>x.GetAccountsByDateRange(Query.FromDate,Query.ToDate,Query.PageNumber,Query.PageSize),Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetPagedEmployerAccountsByDateRangeQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Accounts);
            Assert.IsNotEmpty(actual.Accounts.AccountList);

        }
    }
}
