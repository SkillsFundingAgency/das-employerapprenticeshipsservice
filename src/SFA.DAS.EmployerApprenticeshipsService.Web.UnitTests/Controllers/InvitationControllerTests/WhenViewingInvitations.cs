using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.InvitationControllerTests
{
    public class WhenViewingInvitations : ControllerTestBase
    {
        private Mock<InvitationOrchestrator> _invitationOrchestrator;
        private InvitationController _controller;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();

            _invitationOrchestrator = new Mock<InvitationOrchestrator>(Mediator.Object, Logger.Object);

            _controller = new InvitationController(
                _invitationOrchestrator.Object, _owinWrapper.Object, _featureToggle.Object, _userWhiteList.Object);
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
            _controller = new InvitationController(
                _invitationOrchestrator.Object, _owinWrapper.Object, _featureToggle.Object, _userWhiteList.Object);

            //Act
            var actual = _controller.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirectResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirectResult);
            Assert.AreEqual("Index",actualRedirectResult.RouteValues["action"]);
            Assert.AreEqual("Home",actualRedirectResult.RouteValues["controller"]);
        }

        [Test]
        public async Task ThenTheCorrectInvitationIsRetrieved()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("TEST");

            _invitationOrchestrator.Setup(x => x.GetInvitation(It.Is<string>(i => i == "123")))
                .ReturnsAsync(new OrchestratorResponse<InvitationView> { Data = new InvitationView()});


            _controller = new InvitationController(
                _invitationOrchestrator.Object, _owinWrapper.Object, _featureToggle.Object, _userWhiteList.Object);

            //Act
            var actual = await _controller.View("123");

            //Assert
            _invitationOrchestrator.Verify(x => x.GetInvitation(It.Is<string>(i => i == "123")));
            Assert.IsNotNull(actual);
            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
        }

    }
}
