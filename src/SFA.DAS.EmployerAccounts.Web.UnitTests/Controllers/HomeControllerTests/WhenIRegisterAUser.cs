using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authentication;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenIRegisterAUser
{
    private Mock<HomeOrchestrator> _homeOrchestrator;
    private Mock<EmployerAccountsConfiguration> _configuration;
    private HomeController _homeController;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    protected Mock<HttpContext> MockHttpContext = new();
    protected ControllerContext ControllerContext;
    private const string Schema = "http";
    private const string Authority = "test.local";
    private const string BaseUrl = "https://baseaddress-hyperlink.com";
    private Mock<IUrlActionHelper> _urlActionHelper;

    [SetUp]
    public void Arrange()
    {
        MockHttpContext.Setup(x => x.Request.Host).Returns(new HostString(Authority));
        MockHttpContext.Setup(x => x.Request.Scheme).Returns(Schema);

        _homeOrchestrator = new Mock<HomeOrchestrator>();
        _configuration = new Mock<EmployerAccountsConfiguration>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        _urlActionHelper = new Mock<IUrlActionHelper>();

        _homeController = new HomeController(
            _homeOrchestrator.Object,
            _configuration.Object,
            _flashMessage.Object,
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ILogger<HomeController>>(), null, null, _urlActionHelper.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = MockHttpContext.Object,
            }
        };
    }

    [Test, MoqAutoData]
    public async Task When_GovSignIn_False_CorrelationId_Is_Null_ThenTheUserIsRedirectedToIdams_RegisterLink(string baseUrl)
    {
        //arrange
        _configuration.Object.UseGovSignIn = false;
        _configuration.Object.Identity = GetMockIdentityServerConfiguration(baseUrl);
        var appConstants = new Constants(_configuration.Object.Identity);

        //Act
        var actual = await _homeController.RegisterUser(null);

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual($"{appConstants.RegisterLink()}{Schema}://{Authority}/service/register/new", actualRedirectResult.Url);
    }

    [Test, MoqAutoData]
    public async Task When_GovSignIn_False_CorrelationId_Is_Given_ThenTheUserIsRedirectedToIdams_RegisterLink(
        string baseUrl,
        Guid correlationId)
    {
        //arrange
        _configuration.Object.UseGovSignIn = false;
        _configuration.Object.Identity = GetMockIdentityServerConfiguration(baseUrl);
        var appConstants = new Constants(_configuration.Object.Identity);
        _homeOrchestrator.Setup(x => x.GetProviderInvitation(correlationId)).ReturnsAsync(
            new OrchestratorResponse<ProviderInvitationViewModel>
            {
                Data = null
            });

        //Act
        var actual = await _homeController.RegisterUser(correlationId);

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual($"{appConstants.RegisterLink()}{Schema}://{Authority}/service/register/new", actualRedirectResult.Url);
    }

    [Test, MoqAutoData]
    public async Task When_GovSignIn_False_ProviderInvitation_Is_Given_ThenTheUserIsRedirectedToIdams_RegisterLink(
        string baseUrl,
        Guid correlationId,
        ProviderInvitationViewModel viewModel)
    {
        //arrange
        _configuration.Object.UseGovSignIn = false;
        _configuration.Object.Identity = GetMockIdentityServerConfiguration(baseUrl);
        var appConstants = new Constants(_configuration.Object.Identity);
        _homeOrchestrator.Setup(x => x.GetProviderInvitation(correlationId)).ReturnsAsync(
            new OrchestratorResponse<ProviderInvitationViewModel>
            {
                Data = viewModel
            });

        //Act
        var actual = await _homeController.RegisterUser(correlationId);

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual($"{appConstants.RegisterLink()}{Schema}://{Authority}/service/register/new/{correlationId}" +
                        $"&firstname={WebUtility.UrlEncode(viewModel.EmployerFirstName)}" +
                        $"&lastname={WebUtility.UrlEncode(viewModel.EmployerLastName)}" +
                        $"&email={WebUtility.UrlEncode(viewModel.EmployerEmail)}",
            actualRedirectResult.Url);
    }

    private IdentityServerConfiguration GetMockIdentityServerConfiguration(string baseUrl)
    {
        return new IdentityServerConfiguration
        {
            BaseAddress = baseUrl,
            ClaimIdentifierConfiguration = new ClaimIdentifierConfiguration
            {
                ClaimsBaseUrl = baseUrl,
            },
            RegisterLink = "/register",
            ClientId = "someId"
        };
    }
}