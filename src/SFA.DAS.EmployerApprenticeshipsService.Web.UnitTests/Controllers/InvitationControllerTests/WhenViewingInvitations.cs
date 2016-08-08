using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.InvitationControllerTests
{
    public class WhenViewingInvitations : ControllerTestBase
    {
        private InvitationOrchestrator _invitationOrchestrator;
        private InvitationController _controller;
        private Mock<IOwinWrapper> _owinWrapper;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IOwinWrapper>();

            _invitationOrchestrator = new InvitationOrchestrator(Mediator.Object, Logger.Object);

            _controller = new InvitationController(_invitationOrchestrator, _owinWrapper.Object);
        }

        [Test]
        public void ThenTheUserIsShownTheIndexWhenNotAuthenticated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("");

            //Act
            var actual = _controller.Index();

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ThenTheUserIsRedirectedToTheServiceLandingPageWhenAuthenticated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("my_user_id");
            _controller = new InvitationController(_invitationOrchestrator, _owinWrapper.Object);

            //Act
            var actual = _controller.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirectResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirectResult);
            Assert.AreEqual("Index",actualRedirectResult.RouteValues["action"]);
            Assert.AreEqual("Home",actualRedirectResult.RouteValues["controller"]);
        }
    }
}
