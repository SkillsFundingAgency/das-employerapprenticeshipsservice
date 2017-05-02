using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetPreviousTransactionsCount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPreviousTransactionsCountTests
{
    public class WhenIGetPreviousTransactionsCount : QueryBaseTest<GetPreviousTransactionsCountRequestHandler, GetPreviousTransactionsCountRequest, GetPreviousTransactionsCountResponse>
    {
        private const int TransactionCount = 2;
        private Mock<IDasLevyRepository> _repository;

        public override GetPreviousTransactionsCountRequest Query { get; set; }
        public override GetPreviousTransactionsCountRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPreviousTransactionsCountRequest>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IDasLevyRepository>();
            _repository.Setup(x => x.GetPreviousTransactionsCount(It.IsAny<long>(), It.IsAny<DateTime>()))
                       .ReturnsAsync(TransactionCount);

            base.SetUp();

            Query = new GetPreviousTransactionsCountRequest
            {
                AccountId = 100,
                FromDate = DateTime.Now
            };

            RequestHandler = new GetPreviousTransactionsCountRequestHandler(_repository.Object, RequestValidator.Object);
        }

        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetPreviousTransactionsCount(Query.AccountId, Query.FromDate), Times.Once);
        }

        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(TransactionCount, response.Count);
        }
    }
}
