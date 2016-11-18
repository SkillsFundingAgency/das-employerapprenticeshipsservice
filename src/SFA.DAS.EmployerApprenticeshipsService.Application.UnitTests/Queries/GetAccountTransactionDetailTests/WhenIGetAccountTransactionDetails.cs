using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransactionDetailTests
{
    public class WhenIGetAccountTransactionDetails : QueryBaseTest<GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler, GetAccountTransactionsByDateRangeQuery, GetAccountLevyDeclationTransactionsByDateRangeResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;
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

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetTransactionsByDateRange<LevyDeclarationTransactionLine>(
                It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                           .ReturnsAsync(new List<LevyDeclarationTransactionLine>()
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

            RequestHandler = new GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler(RequestValidator.Object, _dasLevyService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _dasLevyService.Verify(x=>x.GetTransactionsByDateRange<LevyDeclarationTransactionLine>(
                Query.AccountId, Query.FromDate, Query.ToDate, Query.ExternalUserId));
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
