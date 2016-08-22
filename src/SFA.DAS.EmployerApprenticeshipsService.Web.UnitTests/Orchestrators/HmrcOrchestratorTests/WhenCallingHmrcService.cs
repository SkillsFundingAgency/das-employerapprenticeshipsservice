using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.HmrcOrchestratorTests
{
    public class WhenCallingHmrcService
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<ILogger> _logger;
        private Mock<IMediator> _mediator;
        private Mock<ICookieService> _cookieService;
        private Mock<IEmpRefFileBasedService> _empRefFileBasedService;
       
        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();
            _mediator = new Mock<IMediator>();
            _cookieService = new Mock<ICookieService>();
            _empRefFileBasedService = new Mock<IEmpRefFileBasedService>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Hmrc = new HmrcConfiguration { IgnoreDuplicates = false }
            };

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration, _empRefFileBasedService.Object);   
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
                .ReturnsAsync(new GetHmrcEmployerInformationResponse { EmployerLevyInformation = new EmpRefLevyInformation() });
            _empRefFileBasedService.Setup(x => x.GetEmpRef(scenarioUserEmail, It.IsAny<string>()))
                .ReturnsAsync(expectedEmpRef);
            
            //Act
            var result = await _employerAccountOrchestrator.GetHmrcEmployerInformation("123", scenarioUserEmail);
            
            //Assert
            _empRefFileBasedService.Verify(x => x.GetEmpRef(scenarioUserEmail, It.IsAny<string>()), Times.Once);
            Assert.AreEqual(expectedEmpRef, result.Empref);
        }
    }
}
