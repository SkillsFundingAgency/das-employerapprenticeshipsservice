﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SignInUserViewModel = SFA.DAS.EAS.Web.ViewModels.SignInUserViewModel;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIViewTheHomePage
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private HomeController _homeController;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private string ExpectedUserId = "123ABC";
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _owinWrapper = new Mock<IAuthenticationService>();
            _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.RequiresVerification)).Returns("false");

            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _homeOrchestrator.Setup(x => x.GetUsers()).ReturnsAsync(new SignInUserViewModel());
            _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId)).ReturnsAsync(
                new OrchestratorResponse<UserAccountsViewModel>
                {
                    Data = new UserAccountsViewModel
                    {
                        Accounts = new Accounts<Account>
                        {
                            AccountList = new List<Account> {new Account()}
                        }
                    }
                });

            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Identity = new IdentityServerConfiguration
                {
                    BaseAddress = "http://test",
                    ChangePasswordLink = "123",
                    ChangeEmailLink = "123",
                    ClaimIdentifierConfiguration = new ClaimIdentifierConfiguration {ClaimsBaseUrl = "http://claims.test/"}
                }
            };

            _featureToggle = new Mock<IAuthorizationService>();
            _userTestingService = new Mock<IMultiVariantTestingService>();

            _homeController = new HomeController(
                _owinWrapper.Object, _homeOrchestrator.Object, _configuration, _featureToggle.Object,
                _userTestingService.Object,_flashMessage.Object);
        }

        [Test]
        public async Task ThenTheAccountsAreNotReturnedWhenYouAreNotAuthenticated()
        {
            //Act
            await _homeController.Index();

            //Assert
            _homeOrchestrator.Verify(x=>x.GetUserAccounts(It.IsAny<string>()),Times.Never);
        }

        [Test]
        public async Task ThenIfMyAccountIsAuthenticatedButNotActivated()
        {
            //Arrange
            ConfigurationFactory.Current = new IdentityServerConfigurationFactory(
                new EmployerApprenticeshipsServiceConfiguration
                {
                    Identity = new IdentityServerConfiguration { BaseAddress = "http://test.local/identity" ,AccountActivationUrl = "/confirm"}
                });
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);
            _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.RequiresVerification)).Returns("true");

            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirect = actual as RedirectResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual("http://test.local/confirm", actualRedirect.Url);
        }

        [Test]
        public async Task ThenTheAccountsAreReturnedForThatUserWhenAuthenticated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);

            //Act
            await _homeController.Index();

            //Assert
            _homeOrchestrator.Verify(x => x.GetUserAccounts(ExpectedUserId), Times.Once);
        }

        [Test]
        public void ThenTheIndexDoesNotHaveTheAuthorizeAttribute()
        {
            var methods = typeof(HomeController).GetMethods().Where(m => m.Name.Equals("Index")).ToList();

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(true).ToList();

                foreach (var attribute in attributes)
                {
                    var actual = attribute as AuthorizeAttribute;
                    Assert.IsNull(actual);
                }
            }
        }

        [Test]
        public async Task ThenTheUnauthenticatedViewIsReturnedWhenNoUserIsLoggedIn()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("");

            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual("ServiceStartPage", actualViewResult.ViewName);
        }

        [Test]
        public async Task ThenIfIHaveOneAccountIAmRedirectedToTheEmployerTeamsIndexPage()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);

            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual("Index", actualViewResult.RouteValues["Action"].ToString());
            Assert.AreEqual("EmployerTeam", actualViewResult.RouteValues["Controller"].ToString());
        }


        [Test]
        public async Task ThenIfIHaveMoreThanOneAccountIAmRedirectedToTheAccountsIndexPage()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);
            _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId)).ReturnsAsync(
                new OrchestratorResponse<UserAccountsViewModel>
                {
                    Data = new UserAccountsViewModel
                    {
                        Accounts = new Accounts<Account>
                        {
                            AccountList = new List<Account> { new Account(), new Account() }
                        }
                    }
                });

            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual("",actualViewResult.ViewName);
            
        }
    }
}
