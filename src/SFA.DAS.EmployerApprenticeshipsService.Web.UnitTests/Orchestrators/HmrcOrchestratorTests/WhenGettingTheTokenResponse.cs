using System.Threading.Tasks;
using System.Web;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.HmrcOrchestratorTests
{
    public class WhenGettingTheTokenResponse 
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<ILogger> _logger;
        private Mock<IMediator> _mediator;
        private Mock<ICookieService> _cookieService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IEmpRefFileBasedService> _empRefFileBasedService;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();
            _mediator = new Mock<IMediator>();
            _cookieService = new Mock<ICookieService>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Hmrc = new HmrcConfiguration {IgnoreDuplicates = false}
            };
            _empRefFileBasedService = new Mock<IEmpRefFileBasedService>();

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration, _empRefFileBasedService.Object);
        }

        [Test]
        public async Task ThenTheTokenIsRetrievedFromTheQuery()
        {
            //Arrange
            var accessCode = "546tg";
            var returnUrl = "http://someUrl";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetGatewayTokenQuery>()))
                .ReturnsAsync(new GetGatewayTokenQueryResponse {HmrcTokenResponse = new HmrcTokenResponse()});

            //Act
            var token = await _employerAccountOrchestrator.GetGatewayTokenResponse(accessCode, returnUrl, null);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is< GetGatewayTokenQuery>(c=>c.AccessCode.Equals(accessCode) && c.RedirectUrl.Equals(returnUrl))));
            Assert.IsAssignableFrom<HmrcTokenResponse>(token.Data);
        }
    }
}
