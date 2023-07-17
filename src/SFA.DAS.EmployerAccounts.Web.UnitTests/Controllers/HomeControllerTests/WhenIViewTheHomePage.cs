using System.Security.Claims;
using AutoFixture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenIViewTheHomePage : ControllerTestBase
{
    private UserAccountsViewModel _userAccountsViewModel;
    private HomeController _homeController;
    private Mock<HomeOrchestrator> _homeOrchestrator;
    private EmployerAccountsConfiguration _configuration;
    private const string ExpectedUserId = "123ABC";
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private UrlActionHelper _urlActionHelper;
    private GaQueryData _queryData;
    private const string ProfileAddUserDetailsRoute = "https://test.com";

    [SetUp]
    public void Arrange()
    {
        base.Arrange();

        _homeOrchestrator = new Mock<HomeOrchestrator>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        _homeOrchestrator = new Mock<HomeOrchestrator>();

        var fixture = new Fixture();
        _queryData = fixture.Create<GaQueryData>();

        _userAccountsViewModel = SetupUserAccountsViewModel();

        _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, null)).ReturnsAsync(
            new OrchestratorResponse<UserAccountsViewModel>
            {
                Data = _userAccountsViewModel
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
        _urlActionHelper = new UrlActionHelper(_configuration, Mock.Of<IHttpContextAccessor>(), configurationMock.Object);
        _homeController = new HomeController(
            _homeOrchestrator.Object,
            _configuration,
            _flashMessage.Object,
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ILogger<HomeController>>(), configurationMock.Object, null,
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
        await _homeController.Index(null);

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
        var actual = await _homeController.Index(null);

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
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty, new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId), new Claim(DasClaimTypes.RequiresVerification, "true"));
        _homeOrchestrator.Setup(x => x.GetUser(ExpectedUserId)).ReturnsAsync(new User
        {
            FirstName = "test",
            LastName = "Tester"
        });
        
        //Act
        var actual = await _homeController.Index(_queryData);

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
        await _homeController.Index(_queryData);

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
        var actual = await _homeController.Index(null);

        //Assert
        Assert.IsNotNull(actual);
        var actualViewResult = actual as ViewResult;
        Assert.IsNotNull(actualViewResult);
        Assert.AreEqual("ServiceStartPage", actualViewResult.ViewName);
    }

    [Test]
    public async Task ThenIfIHave_OneAccount_IAmRedirectedToTheEmployerTeamsIndexPage()
    {
        //Arrange
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty,
            new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId),
            new Claim(DasClaimTypes.RequiresVerification, "false")
        );

        //Act
        var actual = await _homeController.Index(_queryData);

        //Assert
        Assert.IsNotNull(actual);
        var actualViewResult = actual as RedirectToRouteResult;
        Assert.IsNotNull(actualViewResult);
        Assert.AreEqual(RouteNames.EmployerTeamIndex, actualViewResult.RouteName);
    }

    [Test]
    public async Task ThenIfIHave_OneIncompleteAccount_IAmRedirectedToTheEmployerTeamsIndexPage()
    {
        //Arrange
        AddUserToContext(ExpectedUserId, string.Empty, string.Empty,
            new Claim(ControllerConstants.UserRefClaimKeyName, ExpectedUserId),
            new Claim(DasClaimTypes.RequiresVerification, "false")
        );

        _userAccountsViewModel.Accounts.AccountList[0].NameConfirmed = false;

        //Act
        var actual = await _homeController.Index(_queryData);

        //Assert
        Assert.IsNotNull(actual);
        var actualViewResult = actual as RedirectToRouteResult;
        Assert.IsNotNull(actualViewResult);
        Assert.AreEqual(RouteNames.ContinueNewEmployerAccountTaskList, actualViewResult.RouteName);
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
        var actual = await _homeController.Index(null);

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
        var actual = await _homeController.Index(null);

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
        var actual = await _homeController.Index(null);

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
    public async Task ThenIfIAmAuthenticatedWithNoProfileInformation()
    {
        //Arrange
        var userId = Guid.NewGuid().ToString();
        _configuration.UseGovSignIn = true;
        AddNewGovUserToContext(userId);


        _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>())).ReturnsAsync(
            new OrchestratorResponse<UserAccountsViewModel>
            {
                Data = new UserAccountsViewModel
                {
                    Accounts = new Accounts<Account>()
                }
            });
        _homeOrchestrator.Setup(x => x.GetUser(userId)).ReturnsAsync(new User());

        //Act
        var actual = await _homeController.Index(_queryData);

        //Assert
        Assert.IsNotNull(actual);
        var actualViewResult = actual as RedirectResult;
        Assert.AreEqual($"https://employerprofiles.test-eas.apprenticeships.education.gov.uk/user/add-user-details?_ga={_queryData._ga}&_gl={_queryData._gl}&utm_source={_queryData.utm_source}&utm_campaign={_queryData.utm_campaign}&utm_medium={_queryData.utm_medium}&utm_keywords={_queryData.utm_keywords}&utm_content={_queryData.utm_content}", actualViewResult.Url);
    }
    [Test]
    public async Task ThenIfIAmAuthenticatedWithNoUserInformation()
    {
        //Arrange
        var userId = Guid.NewGuid().ToString();
        _configuration.UseGovSignIn = true;
        AddNewGovUserToContext(userId);


        _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>())).ReturnsAsync(
            new OrchestratorResponse<UserAccountsViewModel>
            {
                Data = new UserAccountsViewModel
                {
                    Accounts = new Accounts<Account>()
                }
            });
        _homeOrchestrator.Setup(x => x.GetUser(userId)).ReturnsAsync((User)null);

        //Act
        var actual = await _homeController.Index(_queryData);

        //Assert
        Assert.IsNotNull(actual);
        var actualViewResult = actual as RedirectResult;
        Assert.AreEqual($"https://employerprofiles.test-eas.apprenticeships.education.gov.uk/user/add-user-details?_ga={_queryData._ga}&_gl={_queryData._gl}&utm_source={_queryData.utm_source}&utm_campaign={_queryData.utm_campaign}&utm_medium={_queryData.utm_medium}&utm_keywords={_queryData.utm_keywords}&utm_content={_queryData.utm_content}", actualViewResult.Url);
    }

    [Test]
    public async Task ThenIfIHaveEmployerUserProfile_ButNoEmployerAccountsAndGovSignInTrueIAmRedirectedToTheProfilePage()
    {
        //Arrange
        _configuration.UseGovSignIn = true;
        AddNewGovUserToContext(ExpectedUserId);
        _homeOrchestrator.Setup(x => x.GetUser(ExpectedUserId)).ReturnsAsync(new User
        {
            FirstName = "test",
            LastName = "Tester"
        });

        _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId, It.IsAny<DateTime?>())).ReturnsAsync(
            new OrchestratorResponse<UserAccountsViewModel>
            {
                Data = new UserAccountsViewModel
                {
                    Accounts = new Accounts<Account>
                    {
                        AccountList = new List<Account>()
                    }
                }
            });

        //Act
        var actual = await _homeController.Index(_queryData);

        //Assert
        Assert.IsNotNull(actual);
        var actualViewResult = actual as RedirectToRouteResult;
        Assert.AreEqual(RouteNames.NewEmpoyerAccountTaskList, actualViewResult.RouteName);
    }

    private static UserAccountsViewModel SetupUserAccountsViewModel()
    {
        return new UserAccountsViewModel
        {
            Accounts = new Accounts<Account>
            {
                AccountList = new List<Account> {
                            new Account
                            {
                                NameConfirmed = true,
                                AccountHistory = new List<AccountHistory>
                                {
                                    new AccountHistory()
                                }
                            }
                        }
            }
        };
    }
}