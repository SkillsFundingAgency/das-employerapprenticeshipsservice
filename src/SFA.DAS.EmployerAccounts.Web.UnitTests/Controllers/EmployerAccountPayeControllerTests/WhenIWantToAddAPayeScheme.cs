using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountPayeControllerTests
{
    public class WhenIWantToAddAPayeScheme
    {
        private Mock<EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
        private Mock<UserAornLockOrchestrator> _userAornLockOrchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private EmployerAccountPayeController _controller;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private const string ExpectedAccountId = "AFD123";
        private const string ExpectedUserId = "456TGF3";

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IAuthenticationService>();
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _employerAccountPayeOrchestrator = new Mock<EmployerAccountPayeOrchestrator>();
            _userAornLockOrchestrator = new Mock<UserAornLockOrchestrator>();

            _userAornLockOrchestrator.Setup(x => x.GetUserAornLockStatus(It.IsAny<string>())).ReturnsAsync(
                new OrchestratorResponse<UserAornPayeStatus>
                {
                    Data = new UserAornPayeStatus
                    {
                        RemainingLock = 0
                    }
                });

            _controller = new EmployerAccountPayeController(
                _owinWrapper.Object, 
                _employerAccountPayeOrchestrator.Object, 
                _userAornLockOrchestrator.Object,
                _featureToggle.Object, 
                _userViewTestingService.Object, 
                _flashMessage.Object);
        }

        [Test]
        public async Task AndISelectGovernmentGatewayThenTheGovernmentGatewayPageIsShown()
        {
            //Act
            var result = await _controller.WaysToAdd(1);

            //Assert
            var redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual(ControllerConstants.GatewayInformActionName, redirectResult.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.EmployerAccountControllerName, redirectResult.RouteValues["controller"].ToString());
        }

        [Test]
        public async Task AndISelectAornThenTheAornPageIsShown()
        {
            //Act
            var result = await _controller.WaysToAdd(2);

            //Assert
            var redirectResult = (RedirectToRouteResult) result;
            Assert.AreEqual(ControllerConstants.SearchUsingAornActionName, redirectResult.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.SearchPensionRegulatorControllerName, redirectResult.RouteValues["controller"].ToString());
        }

        [Test]
        public async Task IDontSelectAnOptionThenAnErrorIsShown()
        {
            //Act
            var result = await _controller.WaysToAdd(0) as ViewResult;

            //Assert
            dynamic model = result.ViewBag;
            Assert.AreEqual(true, model.InError);
        }
    }
}
