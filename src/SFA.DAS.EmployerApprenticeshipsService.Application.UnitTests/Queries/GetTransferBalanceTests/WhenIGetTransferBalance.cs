using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferBalance;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferBalanceTests
{
    public class WhenIGetTransferBalance : QueryBaseTest<GetTransferBalanceRequestHandler, GetTransferBalanaceRequest, GetTransferBalanceResponse>
    {
        private Mock<ITransferRepository> _repository;
        private Mock<ILog> _logger;
        public override GetTransferBalanaceRequest Query { get; set; }
        public override GetTransferBalanceRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetTransferBalanaceRequest>> RequestValidator { get; set; }

        private const string HashedAccountId = "ABC123";
        private const decimal ExpectedTransferBalance = 25300.50M;


        [SetUp]
        public void Arrange()
        {
            SetUp();

            _repository = new Mock<ITransferRepository>();
            _logger = new Mock<ILog>();
            _repository.Setup(x => x.GetTransferBalance(It.IsAny<string>()))
                       .ReturnsAsync(ExpectedTransferBalance);

            Query = new GetTransferBalanaceRequest { HashedAccountId = HashedAccountId };

            RequestHandler = new GetTransferBalanceRequestHandler(_repository.Object, RequestValidator.Object, _logger.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetTransferBalance(HashedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(ExpectedTransferBalance, actual.Balance);
        }
    }
}
