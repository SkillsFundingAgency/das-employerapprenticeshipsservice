﻿using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIModifyMyUserAccount : ControllerTestBase
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private HomeController _homeController;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IAuthenticationService>();
            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _homeController = new HomeController(_owinWrapper.Object, _homeOrchestrator.Object, _configuration, 
                _featureToggle.Object, _userViewTestingService.Object,_flashMessage.Object)
            {
                ControllerContext = _controllerContext.Object
            };
        }
        
        [Test]
        public void ThenThePasswordChangedActionCreatsARedirectToRouteResultToTheIndex()
        {
            //Act
            var actual = _homeController.HandlePasswordChanged();

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<RedirectToRouteResult>(actual);
            var actualRedirect = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual("Index", actualRedirect.RouteValues["action"]);
        }
        
        [Test]
        public async Task ThenIfTheHandleEmailChangedIsCancelledAndTheQueryParamIsSetTheCookieValuesAreNotSet()
        {
            //Act
            await _homeController.HandleEmailChanged(true);

            //Assert
            _flashMessage.Verify(x=>x.Create(It.Is<FlashMessageViewModel>(c=>c.Headline.Equals("You've changed your email")),It.IsAny<string>(),1),Times.Never);
            _owinWrapper.Verify(x => x.UpdateClaims(), Times.Never);
        }


        [Test]
        public void ThenIfTheHandlePasswordChangedIsCancelledAndTheQueryParamIsSetTheCookieValuesAreNotSet()
        {
            //Act
            _homeController.HandlePasswordChanged(true);

            //Assert
            _flashMessage.Verify(x => x.Create(It.Is<FlashMessageViewModel>(c => c.Headline.Equals("You've changed your password")), It.IsAny<string>(), 1), Times.Never);
        }

        [Test]
        public async Task ThenTheAccountCreatedActionCreatesARedirectToRouteResultToTheIndex()
        {
            //Act
            var actual = await _homeController.HandleNewRegistration();

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<RedirectToRouteResult>(actual);
            var actualRedirect = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual("Index", actualRedirect.RouteValues["action"]);
        }

        [Test]
        public async Task ThenTheUserIsUpdatedWhenTheEmailHasChanged()
        {
            //Arrange
            var expectedEmail = "test@test.com";
            var expectedId = "123456";
            var expectedFirstName = "Test";
            var expectedLastName = "tester";
            _owinWrapper.Setup(x => x.GetClaimValue("email")).Returns(expectedEmail);
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(expectedId);
            _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.GivenName)).Returns(expectedFirstName);
            _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.FamilyName)).Returns(expectedLastName);

            //Act
            await _homeController.HandleEmailChanged();

            //Assert
            _owinWrapper.Verify(x => x.UpdateClaims(), Times.Once);
            _homeOrchestrator.Verify(x=>x.SaveUpdatedIdentityAttributes(expectedId,expectedEmail,expectedFirstName,expectedLastName));

        }
    }
}
