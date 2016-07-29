using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIStartTheProcess : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<ICookieService> _cookieService;
        private Mock<IOwinWrapper> _owinWrapper;
        private string ExpectedRedirectUrl = "http://redirect.local.test";
        

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);
            
            _cookieService = new Mock<ICookieService>();

            _orchestrator =  new Mock<EmployerAccountOrchestrator>();
            _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).Returns(ExpectedRedirectUrl);

            _owinWrapper = new Mock<IOwinWrapper>();

            _employerAccountController = new EmployerAccountController (_owinWrapper.Object,_orchestrator.Object, _cookieService.Object);
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
        public void ThenIAmRedirectedToTheGovermentGatewayWhenIConfirmIHaveGatewayCredentials()
        {
            //Act
            var actual = _employerAccountController.Gateway();

            //Assert
            _orchestrator.Verify(x=>x.GetGatewayUrl(It.IsAny<string>()));
            Assert.IsNotNull(actual);
            var actualResult = actual as RedirectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(ExpectedRedirectUrl, actualResult.Url);
        }
    }
}
