using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransactionsTests
{
    public class WhenIGetAccountTransactions : QueryBaseTest<GetAccountTransactionsQueryHandler,GetAccountTransactionsRequest, GetAccountTransactionsResponse>
    {
        private Mock<ITransactionRepository> _repository;
        public override GetAccountTransactionsRequest Query { get; set; }
        public override GetAccountTransactionsQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountTransactionsRequest>> RequestValidator { get; set; }
        private const long ExpectedAccountId = 45648887;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _repository = new Mock<ITransactionRepository>();

            Query = new GetAccountTransactionsRequest {AccountId = ExpectedAccountId, FromDate = DateTime.Now.AddDays(-1), ToDate = DateTime.Now.AddDays(1) };

            RequestHandler = new GetAccountTransactionsQueryHandler(RequestValidator.Object, _repository.Object);
            
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetAccountTransactionsByDateRange(ExpectedAccountId, Query.FromDate, Query.ToDate), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            _repository.Setup(x => x.GetAccountTransactionsByDateRange(ExpectedAccountId, Query.FromDate, Query.ToDate))
                       .ReturnsAsync(new List<TransactionLine> {new TransactionLine()});

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotEmpty(actual.TransactionLines);
        }
    }
}
