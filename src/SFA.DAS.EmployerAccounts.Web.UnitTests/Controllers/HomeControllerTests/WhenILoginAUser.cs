using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenILoginAUser
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private Mock<EmployerAccountsConfiguration> _configuration;
        private HomeController _homeController;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IAuthenticationService>();
            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _configuration = new Mock<EmployerAccountsConfiguration>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _homeController = new HomeController(
                _owinWrapper.Object, _homeOrchestrator.Object, _configuration.Object, _featureToggle.Object, 
                _userViewTestingService.Object,_flashMessage.Object);
        }

        [Test]
        public void ThenTheUserIsRedirectedToTheIndex()
        {
            //Act
            var actual = _homeController.SignIn();
            
            //Assert
            Assert.IsNotNull(actual);
            var actualRedirectResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirectResult);
            Assert.AreEqual("Index",actualRedirectResult.RouteValues["Action"]);

        }
    }
}
