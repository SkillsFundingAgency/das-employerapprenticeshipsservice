using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIStartTheProcess : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private EmployerAccountOrchestrator _orchestrator;
        private Mock<ICookieService> _cookieService;
        private Mock<IOwinWrapper> _owinWrapper;
        private string ExpectedRedirectUrl = "http://redirect.local.test";
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;


        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);
            
            _cookieService = new Mock<ICookieService>();
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();

            _orchestrator =  new EmployerAccountOrchestrator(_mediator.Object, _logger.Object);
            
            _owinWrapper = new Mock<IOwinWrapper>();

            _employerAccountController = new EmployerAccountController (_owinWrapper.Object,_orchestrator, _cookieService.Object);
            _employerAccountController.ControllerContext = _controllerContext.Object;
            _employerAccountController.Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes);
        }

        [Test]
        public void ThenIAmPresentedWithTheEligibilityPage()
        {
            //Act
            var actual = _employerAccountController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(string.Empty,actualViewResult.ViewName);
        }

        [Test]
        public void ThenICanProceedToTheGovernmentGatewayConfirmationPage()
        {
            //Act
            var actual = _employerAccountController.Index(true);

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirectResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirectResult);
            Assert.AreEqual("GovernmentGatewayConfirm",actualRedirectResult.RouteValues["Action"]);
        }

        [Test]
        public async Task ThenIAmRedirectedToTheGovermentGatewayWhenIConfirmIHaveGatewayCredentials()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetGatewayInformationQuery>())).ReturnsAsync(new GetGatewayInformationResponse { Url = ExpectedRedirectUrl });

            //Act
            var actual = await _employerAccountController.Gateway();

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as RedirectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(ExpectedRedirectUrl, actualResult.Url);
        }

        [Test]
        [Ignore("Cant test this without some serious refactoring of the Controller")]
        public async Task ThenTheAccessCodeIsTakenFromTheUrlExchangedForThe()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetGatewayTokenQuery>())).ReturnsAsync(new GetGatewayTokenQueryResponse() { HmrcTokenResponse = new HmrcTokenResponse()});

            //Act
            var actual = await _employerAccountController.GateWayResponse();
        }
    }
}
