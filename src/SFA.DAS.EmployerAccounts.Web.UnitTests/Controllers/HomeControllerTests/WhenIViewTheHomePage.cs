using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenIViewTheHomePage : ControllerTestBase
{
    private HomeController _homeController;
    private Mock<HomeOrchestrator> _homeOrchestrator;
    private EmployerAccountsConfiguration _configuration;
    private const string ExpectedUserId = "123ABC";
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private UrlActionHelper _urlActionHelper;
    private const string ProfileAddUserDetailsRoute = "https://test.com";

    [SetUp]
    public void Arrange()
    {
        base.Arrange();

        _homeOrchestrator = new Mock<HomeOrchestrator>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
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
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        _urlActionHelper = new UrlActionHelper(_configuration, Mock.Of<IHttpContextAccessor>(),configurationMock.Object);
        _homeController = new HomeController(
            _homeOrchestrator.Object,
            _configuration,
            _flashMessage.Object,
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ILogger<HomeController>>(), null, null,
            _urlActionHelper)
        {
            ControllerContext = new ControllerContext { HttpContext = MockHttpContext.Object },
            Url = new UrlHelper(new ActionContext(Mock.Of<HttpContext>(), new RouteData(), new ActionDescriptor()))
        };
    }

    [Test]
    public async Task ThenTheAccountsAreNotReturnedWhenYouAreNotAuthenticated()
    {
        //Arrange
        AddEmptyUserToContext();

        //Act
        await _homeController.Index();

        //Assert
        _homeOrchestrator.Verify(x => x.GetUserAccounts(It.Is<string>(c => c.Equals(string.Empty)), It.IsAny<DateTime?>()), Times.Never);
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
        AddUserToContext(new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId), new Claim(DasClaimTypes.RequiresVerification, "true"));

        //Act
        var actual = await _homeController.Index();

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirect = actual as RedirectResult;
        Assert.IsNotNull(actualRedirect);
        Assert.AreEqual("http://test.local/confirm", actualRedirect.Url);
    }
    
    [Test]
    public async Task ThenIfMyAccountIsAuthenticatedButNotActivatedForGovThenIgnoredAndGoesToIndex()
    {
        //Arrange
        _configuration.UseGovSignIn = true;
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty,new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId), new Claim(DasClaimTypes.RequiresVerification, "true"));

        //Act
        var actual = await _homeController.Index();

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirect = actual as RedirectToRouteResult;
        Assert.IsNotNull(actualRedirect);
        Assert.AreEqual("employer-team-index", actualRedirect.RouteName);
    }

    [Test]
    public async Task ThenTheAccountsAreReturnedForThatUserWhenAuthenticated()
    {
        //Arrange
        
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty,
            new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId),
            new Claim(DasClaimTypes.RequiresVerification, "false")
        );

        //Act
        await _homeController.Index();

        //Assert
        _homeOrchestrator.Verify(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>()), Times.Once);
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
        AddEmptyUserToContext();

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
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty,
            new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId),
            new Claim(DasClaimTypes.RequiresVerification, "false")
        );

        //Act
        var actual = await _homeController.Index();

        //Assert
        Assert.IsNotNull(actual);
        var actualViewResult = actual as RedirectToRouteResult;
        Assert.IsNotNull(actualViewResult);
        Assert.AreEqual(RouteNames.EmployerTeamIndex, actualViewResult.RouteName);
    }


    [Test]
    public async Task ThenIfIHaveMoreThanOneAccountIAmRedirectedToTheAccountsIndexPage()
    {
        //Arrange
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty,
            new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId),
            new Claim(DasClaimTypes.RequiresVerification, "false")
        );

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
        var actualViewResult = actual as ViewResult;
        Assert.IsNotNull(actualViewResult);
        Assert.AreEqual(null, actualViewResult.ViewName);
    }

    [Test]
    public async Task ThenIfIHaveMoreThanOneAccountIAmRedirectedToTheAccountsIndexPage_WithTermsAndConditionBannerDisplayed()
    {
        //Arrange
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty,
            new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId),
            new Claim(DasClaimTypes.RequiresVerification, "false")
        );

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
        var actualViewResult = actual as ViewResult;
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
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty,
            new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId),
            new Claim(DasClaimTypes.RequiresVerification, "false")
        );

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
        var actualViewResult = actual as ViewResult;
        Assert.IsNotNull(actualViewResult);

        var viewModel = actualViewResult.Model;
        Assert.IsInstanceOf<OrchestratorResponse<UserAccountsViewModel>>(viewModel);
        var userAccountsViewModel = (OrchestratorResponse<UserAccountsViewModel>)viewModel;

        Assert.AreEqual(false, userAccountsViewModel.Data.ShowTermsAndConditionBanner);
    }

    [Test]
    public async Task ThenIfIHaveNoAccountsAndGovSignInTrueIAmRedirectedToTheProfilePage()
    {
        //Arrange
        _configuration.UseGovSignIn = true;
        AddEmptyUserToContext();
        

        _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>())).ReturnsAsync(
            new OrchestratorResponse<UserAccountsViewModel>
            {
                Data = new UserAccountsViewModel
                {
                    Accounts = new Accounts<Account>()
                }
            });

        //Act
        var actual = await _homeController.Index();

        //Assert
        Assert.IsNotNull(actual);
        var actualViewResult = actual as RedirectResult;
        Assert.AreEqual( "https://employerprofiles.test-eas.apprenticeships.education.gov.uk/user/add-user-details", actualViewResult.Url);
    }
}