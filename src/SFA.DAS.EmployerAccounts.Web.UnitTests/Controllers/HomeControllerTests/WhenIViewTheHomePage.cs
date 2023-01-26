﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SignInUserViewModel = SFA.DAS.EmployerAccounts.Web.ViewModels.SignInUserViewModel;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.NLog.Logger;
using System;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIViewTheHomePage
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private HomeController _homeController;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private EmployerAccountsConfiguration _configuration;
        private string ExpectedUserId = "123ABC";     
        private Mock<IMultiVariantTestingService> _userTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<IDependencyResolver> _dependancyResolver;


        [SetUp]
        public void Arrange()
        {
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _owinWrapper = new Mock<IAuthenticationService>();
            _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.RequiresVerification)).Returns("false");

            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, null)).ReturnsAsync(
                new OrchestratorResponse<UserAccountsViewModel>
                {
                    Data = new UserAccountsViewModel
                    {
                        Accounts = new Accounts<Account>
                        {
                            AccountList = new List<Account> { new Account() }
                        }
                    }
                });

            _configuration = new EmployerAccountsConfiguration
            {
                Identity = new IdentityServerConfiguration
                {
                    BaseAddress = "http://test",
                    ChangePasswordLink = "123",
                    ChangeEmailLink = "123",
                    ClaimIdentifierConfiguration = new ClaimIdentifierConfiguration { ClaimsBaseUrl = "http://claims.test/" }
                },
                EmployerPortalBaseUrl = "https://localhost"
            };

            _dependancyResolver = new Mock<IDependencyResolver>();
            _dependancyResolver.Setup(r => r.GetService(typeof(EmployerAccountsConfiguration))).Returns(_configuration);

            DependencyResolver.SetResolver(_dependancyResolver.Object);
         
            _userTestingService = new Mock<IMultiVariantTestingService>();

            _homeController = new HomeController(
                _owinWrapper.Object, 
                _homeOrchestrator.Object,      
                _configuration, 
                _userTestingService.Object, 
                _flashMessage.Object,
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ILog>())
            {
                Url = new UrlHelper()
            };
        }

        [Test]
        public async Task ThenTheAccountsAreNotReturnedWhenYouAreNotAuthenticated()
        {
            //Act
            await _homeController.Index();

            //Assert
            _homeOrchestrator.Verify(x => x.GetUserAccounts(It.IsAny<string>(), It.IsAny<DateTime?>()), Times.Never);
        }

        [Test]
        public async Task ThenIfMyAccountIsAuthenticatedButNotActivated()
        {
            //Arrange
            ConfigurationFactory.Current = new IdentityServerConfigurationFactory(
                new EmployerAccountsConfiguration
                {
                    Identity = new IdentityServerConfiguration { BaseAddress = "http://test.local/identity", AccountActivationUrl = "/confirm" }
                });
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);
            _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.RequiresVerification)).Returns("true");

            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirect = actual as Microsoft.AspNetCore.Mvc.RedirectResult;
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
            _homeOrchestrator.Verify(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>()), Times.Once);
        }

        [Test]
        public async Task ThenTheClaimsAreRefreshedForThatUserWhenAuthenticated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);

            //Act
            await _homeController.Index();

            //Assert
            _owinWrapper.Verify(x => x.UpdateClaims(), Times.Once);
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
                    var actual = attribute as Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
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
            var actualViewResult = actual as Microsoft.AspNetCore.Mvc.ViewResult;
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
            _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>())).ReturnsAsync(
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
            var actualViewResult = actual as Microsoft.AspNetCore.Mvc.ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual("", actualViewResult.ViewName);
        }

        [Test]
        public async Task ThenIfIHaveMoreThanOneAccountIAmRedirectedToTheAccountsIndexPage_WithTermsAndConditionBannerDisplayed()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);
            _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>())).ReturnsAsync(
                new OrchestratorResponse<UserAccountsViewModel>
                {
                    Data = new UserAccountsViewModel
                    {
                        Accounts = new Accounts<Account>
                        {
                            AccountList = new List<Account> { new Account(), new Account() }
                        },

                        LastTermsAndConditionsUpdate = DateTime.Now,
                        TermAndConditionsAcceptedOn = DateTime.Now.AddDays(-20)
                    }
                });

            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as Microsoft.AspNetCore.Mvc.ViewResult;
            Assert.IsNotNull(actualViewResult);

            var viewModel = actualViewResult.Model;
            Assert.IsInstanceOf<OrchestratorResponse<UserAccountsViewModel>>(viewModel);
            var userAccountsViewModel = (OrchestratorResponse<UserAccountsViewModel>)viewModel;

            Assert.AreEqual(true, userAccountsViewModel.Data.ShowTermsAndConditionBanner);
        }

        [Test]
        public async Task ThenIfIHaveMoreThanOneAccountIAmRedirectedToTheAccountsIndexPage_WithTermsAndConditionBannerNotDisplayed()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);
            _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>())).ReturnsAsync(
                new OrchestratorResponse<UserAccountsViewModel>
                {
                    Data = new UserAccountsViewModel
                    {
                        Accounts = new Accounts<Account>
                        {
                            AccountList = new List<Account> { new Account(), new Account() }
                        },

                        LastTermsAndConditionsUpdate = DateTime.Now.AddDays(-20),
                        TermAndConditionsAcceptedOn = DateTime.Now
                    }
                });

            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as Microsoft.AspNetCore.Mvc.ViewResult;
            Assert.IsNotNull(actualViewResult);

            var viewModel = actualViewResult.Model;
            Assert.IsInstanceOf<OrchestratorResponse<UserAccountsViewModel>>(viewModel);
            var userAccountsViewModel = (OrchestratorResponse<UserAccountsViewModel>)viewModel;

            Assert.AreEqual(false, userAccountsViewModel.Data.ShowTermsAndConditionBanner);
        }
    }
}
