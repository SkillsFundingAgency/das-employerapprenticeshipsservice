using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIRemoveAnAgreement
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private EmployerAgreementOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _logger.Object, _configuration);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithTheListOfOrganisations()
        {
            //Arrange
            var hashedAgreementId = "123RFAD";

            //Act
            await _orchestrator.GetLegalAgreementsToRemove(hashedAgreementId);
        }

        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
        {
            
        }

        [Test]
        public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
        {
            
        }

        [Test]
        public async Task ThenTheValuesAreReturnedInTheResponseFromTheMediatorCall()
        {
            
        }

        [Test]
        public async Task ThenWhenIRemoveAnAgreementTheMediatorCallIsMade()
        {
            
        }

        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownWhenRemovingTheAgreementItIsReturnedInTheOrchestratorResponse()
        {
            
        }

        [Test]
        public async Task ThenIfAnUnauthorizedAccessExceptionIsThrownWhenRemovingTheAgreementItIsReturnedInTheOrchestratorResponse()
        {
            
        }
    }
}
