using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.DeleteApprentice;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenDeletingApprenticeship
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IHashingService> _hashingService;
        private Mock<ICommitmentStatusCalculator> _calculator;

        private EmployerCommitmentsOrchestrator _employerCommitmentOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _calculator = new Mock<ICommitmentStatusCalculator>();

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            _hashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            _hashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            _employerCommitmentOrchestrator = new EmployerCommitmentsOrchestrator(_mediator.Object,
                _hashingService.Object, _calculator.Object, _logger.Object);
        }

        [Test]
        public async Task ShouldCallMediatorToDelete()
        {
            //Arrange
            var model = new DeleteApprenticeshipConfirmationViewModel
            {
                HashedAccountId = "ABC123",
                HashedCommitmentId = "ABC321",
                HashedApprenticeshipId = "ABC456"
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<DeleteApprenticeshipCommand>())).ReturnsAsync(new Unit());

            //Act
            await _employerCommitmentOrchestrator.DeleteApprenticeship(model);

            //Assert
            _mediator.Verify(x=> x.SendAsync(It.Is<DeleteApprenticeshipCommand>(c=> c.AccountId == 123L && c.ApprenticeshipId == 456L)), Times.Once);
        }

        [Test]
        public async Task ShouldReturnTheApprenticeshipName()
        {
            //Arrange
            var model = new DeleteApprenticeshipConfirmationViewModel
            {
                HashedAccountId = "ABC123",
                HashedCommitmentId = "ABC321",
                HashedApprenticeshipId = "ABC456"
            };

            var expected = new Apprenticeship
            {
                FirstName = "John",
                LastName = "Smith"
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = expected
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<DeleteApprenticeshipCommand>())).ReturnsAsync(new Unit());

            //Act
            var result = await _employerCommitmentOrchestrator.DeleteApprenticeship(model);

            //Assert
            Assert.AreEqual(expected.ApprenticeshipName, result);
        }
    }
}
