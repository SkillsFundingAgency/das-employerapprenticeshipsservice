using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransactionDetailTests
{
    public class WhenIGetAccountTransactionDetails : QueryBaseTest<GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler, GetAccountTransactionsByDateRangeQuery, GetAccountLevyDeclationTransactionsByDateRangeResponse>
    {
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private DateTime _fromDate;
        private DateTime _toDate;
        private long _accountId;
        public override GetAccountTransactionsByDateRangeQuery Query { get; set; }
        public override GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountTransactionsByDateRangeQuery>> RequestValidator { get; set; }
      
        [SetUp]
        public void Arrange()
        {
            _fromDate = DateTime.Now.AddDays(-10);
            _toDate = DateTime.Now.AddDays(-2);
            _accountId = 1;

            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _dasLevyRepository.Setup(x => x.GetTransactionsByDateRange(
                    It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                            .ReturnsAsync(new List<TransactionLine>
                            {
                                new LevyDeclarationTransactionLine()
                            });

            Query = new GetAccountTransactionsByDateRangeQuery
            {
                AccountId = _accountId,
                FromDate= _fromDate,
                ToDate = _toDate,
                ExternalUserId = "ABC123"
            };

            SetUp();

            RequestHandler = new GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler(RequestValidator.Object, _dasLevyRepository.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _dasLevyRepository.Verify(x=>x.GetTransactionsByDateRange(Query.AccountId, Query.FromDate, Query.ToDate));
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
    }
}
