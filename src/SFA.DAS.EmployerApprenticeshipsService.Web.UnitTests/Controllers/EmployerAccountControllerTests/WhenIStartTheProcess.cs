using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIStartTheProcess
    {
        private EmployerAccountController _employerAccountController;
        private EmployerAccountOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICookieService> _cookieService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _cookieService = new Mock<ICookieService>();
            _orchestrator = new EmployerAccountOrchestrator(_mediator.Object);
            _employerAccountController = new EmployerAccountController(_orchestrator, _cookieService.Object);
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
    }
}
