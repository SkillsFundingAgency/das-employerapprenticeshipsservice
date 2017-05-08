using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransactionDetailTests
{
    public class WhenIGetAccountTransactionDetails : QueryBaseTest<GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler, GetAccountTransactionsByDateRangeQuery, GetAccountLevyDeclationTransactionsByDateRangeResponse>
    {
        private Mock<ITransactionRepository> _transactionRepository;
        private DateTime _fromDate;
        private DateTime _toDate;
        private long _accountId;
        private Mock<IHmrcDateService> _hmrcDataService;
        public override GetAccountTransactionsByDateRangeQuery Query { get; set; }
        public override GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountTransactionsByDateRangeQuery>> RequestValidator { get; set; }
      
        [SetUp]
        public void Arrange()
        {
            _hmrcDataService = new Mock<IHmrcDateService>();

            _fromDate = DateTime.Now.AddDays(-10);
            _toDate = DateTime.Now.AddDays(-2);
            _accountId = 1;

            _transactionRepository = new Mock<ITransactionRepository>();
            _transactionRepository.Setup(x => x.GetTransactionDetailsByDateRange(
                    It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                            .ReturnsAsync(new List<TransactionLine>
                            {
                                new LevyDeclarationTransactionLine {PayrollMonth = 1,PayrollYear = "16-17"}
                            });

            Query = new GetAccountTransactionsByDateRangeQuery
            {
                AccountId = _accountId,
                FromDate= _fromDate,
                ToDate = _toDate,
                ExternalUserId = "ABC123"
            };

            SetUp();

            RequestHandler = new GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler(RequestValidator.Object, _transactionRepository.Object, _hmrcDataService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _transactionRepository.Verify(x=>x.GetTransactionDetailsByDateRange(Query.AccountId, Query.FromDate, Query.ToDate));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual.Transactions);
        }

        [Test]
        public async Task ThenThePayrollDateIsCorrectlyMapped()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _hmrcDataService.Verify(x=>x.GetDateFromPayrollYearMonth(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }
}
