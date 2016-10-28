using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetGatewayInformation;
using SFA.DAS.EAS.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.HmrcOrchestratorTests
{
    public class WhenCallingHmrcService
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<ILogger> _logger;
        private Mock<IMediator> _mediator;
        private Mock<ICookieService> _cookieService;

        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();
            _mediator = new Mock<IMediator>();
            _cookieService = new Mock<ICookieService>();
            
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Hmrc = new HmrcConfiguration()
            };

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);   
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

        [Test]
        public async Task ThenICanRetrieveEmployerInformationWithMyAccessToken()
        {
            //Arrange
            var expectedAuthToken = "123";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetHmrcEmployerInformationQuery>())).ReturnsAsync(new GetHmrcEmployerInformationResponse { EmployerLevyInformation = new EmpRefLevyInformation()});

            //Act
            await _employerAccountOrchestrator.GetHmrcEmployerInformation(expectedAuthToken, string.Empty);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetHmrcEmployerInformationQuery>(c => c.AuthToken.Equals(expectedAuthToken))));
        }

        [Test]
        public async Task ThenICanRetrieveCorrectEmpRefOfScenarioUser()
        {
            //Arrange
            var scenarioUserEmail = "test.user@test.com";
            var expectedEmpRef = "123/456789";

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetHmrcEmployerInformationQuery>()))
                .ReturnsAsync(new GetHmrcEmployerInformationResponse { EmployerLevyInformation = new EmpRefLevyInformation(), Empref = expectedEmpRef});
          
            //Act
            var result = await _employerAccountOrchestrator.GetHmrcEmployerInformation("123", scenarioUserEmail);
            
            //Assert
            Assert.AreEqual(expectedEmpRef, result.Empref);
        }
    }
}
