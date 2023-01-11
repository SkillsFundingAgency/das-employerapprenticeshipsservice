using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.NLog.Logger;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIViewTermsAndCondition : ControllerTestBase
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private EmployerAccountsConfiguration _configuration;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private HomeController _homeController;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrage()
        {
            base.Arrange();

            _owinWrapper = new Mock<IAuthenticationService>();
            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _configuration = new EmployerAccountsConfiguration();

            _homeController = new HomeController(
                _owinWrapper.Object,
                _homeOrchestrator.Object,
                _configuration,
                _userViewTestingService.Object,
                _flashMessage.Object,
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ILog>())
            {
                ControllerContext = _controllerContext.Object
            };
        }

        [Test]
        public void ThenTheViewIsReturned()
        {
            //Act
            var actual = _homeController.TermsAndConditions("returnUrl", "hashedId");

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<ViewResult>(actual);
        }

        [Test]
        public void ThenTheViewModelIsMappedCorrectly()
        {
            //Act
            var result = _homeController.TermsAndConditions("returnUrl", "hashedId");

            //Assert
            var viewResult = (ViewResult)result;
            var viewModel = viewResult.Model;

            Assert.IsInstanceOf<TermsAndConditionsNewViewModel>(viewModel);
            var termsAndConditionViewModel = (TermsAndConditionsNewViewModel)viewModel;

            Assert.AreEqual("returnUrl", termsAndConditionViewModel.ReturnUrl);
            Assert.AreEqual("hashedId", termsAndConditionViewModel.HashedAccountId);
        }


        [Test]
        public async Task ThenIsRedirectedToEmployerTeamController()
        {
            var termsAndConditionViewModel = new TermsAndConditionsNewViewModel() { HashedAccountId = "HashedId", ReturnUrl = "EmployerTeam" };
            //Act
            var result = await _homeController.TermsAndConditions(termsAndConditionViewModel);

            //Assert
            var redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Index", redirectResult.RouteValues["action"].ToString());
            Assert.AreEqual("EmployerTeam", redirectResult.RouteValues["controller"].ToString());
        }

        [Test]
        public async Task ThenIsRedirectedToHomeController()
        {
            var termsAndConditionViewModel = new TermsAndConditionsNewViewModel() { HashedAccountId = "HashedId", ReturnUrl = "Home" };
            //Act
            var result = await _homeController.TermsAndConditions(termsAndConditionViewModel);

            //Assert
            var redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Index", redirectResult.RouteValues["action"].ToString());
        }
    }
}
