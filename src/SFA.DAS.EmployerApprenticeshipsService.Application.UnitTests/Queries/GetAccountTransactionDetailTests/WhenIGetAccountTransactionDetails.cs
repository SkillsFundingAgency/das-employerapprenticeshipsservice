using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransactionDetailTests
{
    public class WhenIGetAccountTransactionDetails : QueryBaseTest<GetAccountTransactionDetailQueryHandler, GetAccountTransactionDetailQuery, GetAccountTransactionDetailResponse>
    {
        private Mock<IDasLevyRepository> _repository;
        public override GetAccountTransactionDetailQuery Query { get; set; }
        public override GetAccountTransactionDetailQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountTransactionDetailQuery>> RequestValidator { get; set; }
        private const long ExpectedId = 123124;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IDasLevyRepository>();
            _repository.Setup(x => x.GetTransactionDetail(ExpectedId)).ReturnsAsync(new List<TransactionLineDetail> { new TransactionLineDetail() });

            Query = new GetAccountTransactionDetailQuery {Id= ExpectedId};

            SetUp();
            RequestHandler = new GetAccountTransactionDetailQueryHandler(RequestValidator.Object, _repository.Object);
            
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetTransactionDetail(It.IsAny<long>()));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual.Data);
        }
    }
}
