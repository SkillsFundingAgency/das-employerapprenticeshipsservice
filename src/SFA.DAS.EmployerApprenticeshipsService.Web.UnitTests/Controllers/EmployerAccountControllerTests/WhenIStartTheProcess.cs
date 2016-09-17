using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIStartTheProcess : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private string ExpectedRedirectUrl = "http://redirect.local.test";
        

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);
            
            new Mock<ICookieService>();
            
            _orchestrator =  new Mock<EmployerAccountOrchestrator>();
            
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();

            _employerAccountController = new EmployerAccountController (_owinWrapper.Object,_orchestrator.Object, _featureToggle.Object);
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
            _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(ExpectedRedirectUrl);

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
            
            //_mediator.Setup(x => x.SendAsync(It.IsAny<GetGatewayTokenQuery>())).ReturnsAsync(new GetGatewayTokenQueryResponse() { HmrcTokenResponse = new HmrcTokenResponse()});

            //Act
            var actual = await _employerAccountController.GateWayResponse();
        }
    }
}
