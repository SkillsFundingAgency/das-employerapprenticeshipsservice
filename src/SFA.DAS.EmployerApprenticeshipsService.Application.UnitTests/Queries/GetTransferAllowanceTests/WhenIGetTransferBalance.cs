using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferAllowanceTests
{
    public class WhenIGetTransferBalance : QueryBaseTest<GetTransferAllowanceRequestHandler, GetTransferAllowanceRequest, GetTransferAllowanceResponse>
    {
        private Mock<ITransferRepository> _repository;
        private Mock<IHashingService> _hashngService;
        private Mock<ILog> _logger;
        public override GetTransferAllowanceRequest Query { get; set; }
        public override GetTransferAllowanceRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetTransferAllowanceRequest>> RequestValidator { get; set; }

        private const string HashedAccountId = "ABC123";
        private const long AccountId = 1234;
        private const decimal ExpectedTransferBalance = 25300.50M;


        [SetUp]
        public void Arrange()
        {
            SetUp();

            _repository = new Mock<ITransferRepository>();
            _hashngService = new Mock<IHashingService>();
            _logger = new Mock<ILog>();

            _repository.Setup(x => x.GetTransferAllowance(It.IsAny<long>()))
                       .ReturnsAsync(ExpectedTransferBalance);

            _hashngService.Setup(x => x.DecodeValue(It.IsAny<string>()))
                          .Returns(AccountId);

            Query = new GetTransferAllowanceRequest { HashedAccountId = HashedAccountId };

            RequestHandler = new GetTransferAllowanceRequestHandler(_repository.Object, _hashngService.Object, RequestValidator.Object, _logger.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _hashngService.Verify(x => x.DecodeValue(HashedAccountId), Times.Once);
            _repository.Verify(x => x.GetTransferAllowance(AccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(ExpectedTransferBalance, actual.Balance);
        }

        [Test]
        public void ThenIfUserIsUnauthorisedTheyShouldNotGetABalance()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetTransferAllowanceRequest>()))
                .ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act + Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => RequestHandler.Handle(Query));

            _hashngService.Verify(x => x.DecodeValue(It.IsAny<string>()), Times.Never);
            _repository.Verify(x => x.GetTransferAllowance(It.IsAny<long>()), Times.Never);
            RequestValidator.Verify(x => x.ValidateAsync(Query), Times.Once);
        }

    }
}
