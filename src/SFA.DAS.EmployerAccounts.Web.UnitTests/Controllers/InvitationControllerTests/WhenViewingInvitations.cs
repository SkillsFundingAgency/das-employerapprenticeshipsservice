using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.InvitationControllerTests
{
    public class WhenViewingInvitations : ControllerTestBase
    {
        private Mock<InvitationOrchestrator> _invitationOrchestrator;
        private InvitationController _controller;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private EmployerAccountsConfiguration _configuration;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IAuthenticationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _invitationOrchestrator = new Mock<InvitationOrchestrator>();

            _configuration = new EmployerAccountsConfiguration();

            _controller = new InvitationController(
                _invitationOrchestrator.Object, _owinWrapper.Object, 
                _userViewTestingService.Object, _configuration, _flashMessage.Object);
        }

        [Test]
        public void ThenTheUserIsShownTheIndexWhenNotAuthenticated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("");

            //Act
            var actual = _controller.Invite();

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ThenTheUserIsRedirectedToTheServiceLandingPageWhenAuthenticated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("my_user_id");
            
            //Act
            var actual = _controller.Invite();

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
            

            //Act
            var actual = await _controller.Details("123");

            //Assert
            _invitationOrchestrator.Verify(x => x.GetInvitation(It.Is<string>(i => i == "123")));
            Assert.IsNotNull(actual);
            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
        }

    }
}
