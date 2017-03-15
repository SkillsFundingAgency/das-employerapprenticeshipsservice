using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.AgreementOrchestratorTests
{
    internal class WhenIGetAnAgreement
    {
        private AgreementOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private EmployerAgreementView _agreement;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _agreement = new EmployerAgreementView();

            var response = new GetEmployerAgreementByIdResponse()
            {
                EmployerAgreement = _agreement
            };

            _orchestrator = new AgreementOrchestrator(_mediator.Object, _logger.Object);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAgreementByIdRequest>()))
                .ReturnsAsync(response);
        }

        [Test]
        public async Task ThenARequestShouldBeCreatedAndItsResponseReturned()
        {
            //Arrange
            var hashedAgreementId = "ABC123";

            //Act
            var result = await _orchestrator.GetAgreement(hashedAgreementId);

            //Assert
            Assert.AreEqual(_agreement, result.Data);
        }
    }
}
