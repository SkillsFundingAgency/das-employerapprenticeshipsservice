using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenILoginAUser
{
    private Mock<HomeOrchestrator> _homeOrchestrator;
    private Mock<EmployerAccountsConfiguration> _configuration;
    private HomeController _homeController;    
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

    [SetUp]
    public void Arrange()
    {
        _homeOrchestrator = new Mock<HomeOrchestrator>();
        _configuration = new Mock<EmployerAccountsConfiguration>();          
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _homeController = new HomeController(
            _homeOrchestrator.Object, 
            _configuration.Object, 
            _flashMessage.Object,
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ILogger<HomeController>>(),
            Mock.Of<IMultiVariantTestingService>());
    }

    [Test]
    public void ThenTheUserIsRedirectedToTheIndex()
    {
        //Act
        var actual = _homeController.SignIn();
            
        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectToActionResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual("Index", actualRedirectResult.ActionName);
    }
}