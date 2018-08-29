﻿using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.InvitationControllerTests
{
    public class WhenViewingInvitations : ControllerTestBase
    {
        private Mock<InvitationOrchestrator> _invitationOrchestrator;
        private InvitationController _controller;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _invitationOrchestrator = new Mock<InvitationOrchestrator>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _controller = new InvitationController(
                _invitationOrchestrator.Object, _owinWrapper.Object, _featureToggle.Object, 
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
