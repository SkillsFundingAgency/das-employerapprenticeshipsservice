using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests
{
    public class WhenCallingHmrcService
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<ILogger> _logger;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();
            _mediator = new Mock<IMediator>();

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object);   
        }

        [Test]
        public async Task ThenTheHmrcServiceIsCalled()
        {
            //Arrange
            var redirectUrl = "myUrl";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetGatewayInformationQuery>())).ReturnsAsync(new GetGatewayInformationResponse {Url = "someurl"});

            //Act
            await _employerAccountOrchestrator.GetGatewayUrl(redirectUrl);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetGatewayInformationQuery>(c=>c.ReturnUrl.Equals(redirectUrl))));
        }
    }
}
