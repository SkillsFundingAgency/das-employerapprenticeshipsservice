using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenIComeInFromSaveAndSearch : ControllerTestBase
{
    private HomeController _homeController;
    private EmployerAccountsConfiguration _configuration;
    private Mock<HomeOrchestrator> _homeOrchestrator;
    private Mock<ICookieStorageService<ReturnUrlModel>> _returnUrlCookieStorageService;
    private Mock<IUrlActionHelper> _urlActionHelper;

    private const string ExpectedEmail = "test@test.com";
    private const string ExpectedId = "123456";
    private const string ExpectedFirstName = "Test";
    private const string ExpectedLastName = "tester";
    private const string ExpectedReturnUrl = "campaign page";
    private readonly string _expectedCorrelationId = null;
    private IActionResult _actualResult;

    [SetUp]
    public async Task Arrange()
    {
        base.Arrange();
        AddUserToContext(ExpectedId, ExpectedEmail, ExpectedFirstName, ExpectedLastName);

        _configuration = new EmployerAccountsConfiguration();
        _homeOrchestrator = new Mock<HomeOrchestrator>();
        _returnUrlCookieStorageService = new Mock<ICookieStorageService<ReturnUrlModel>>();
        _urlActionHelper = new Mock<IUrlActionHelper>();

        _homeController = new HomeController(
            _homeOrchestrator.Object,
            _configuration,
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            _returnUrlCookieStorageService.Object,
                Mock.Of<ILogger<HomeController>>(), null, null, 
            _urlActionHelper.Object)
        {
            ControllerContext = ControllerContext
        };

        _actualResult = await _homeController.SaveAndSearch(ExpectedReturnUrl);
    }

    [Test]
    public void ThenTheUpdatedIdentityAttributesAreSaved()
    {
        _homeOrchestrator.Verify(x => x.SaveUpdatedIdentityAttributes(ExpectedId, ExpectedEmail, ExpectedFirstName, ExpectedLastName, _expectedCorrelationId));
    }

    [Test]
    public void ThenTheReturnUrlIsStoredInTheCookieService()
    {
        _returnUrlCookieStorageService.Verify(x =>
            x.Create(It.Is<ReturnUrlModel>(model => model.Value == ExpectedReturnUrl), HomeController.ReturnUrlCookieName, It.IsAny<int>()
            ), Times.Once);
    }

    [Test]
    public void ThenTheRedirectIsToGetApprenticeshipFunding()
    {
        Assert.That(_actualResult, Is.Not.Null);
        Assert.That(_actualResult, Is.AssignableFrom<RedirectToActionResult>());
        var actualRedirect = _actualResult as RedirectToActionResult;
        Assert.That(actualRedirect, Is.Not.Null);
        Assert.That(actualRedirect.ControllerName, Is.EqualTo(ControllerConstants.EmployerAccountControllerName));
        Assert.That(actualRedirect.ActionName, Is.EqualTo(ControllerConstants.GetApprenticeshipFundingActionName));
    }
}