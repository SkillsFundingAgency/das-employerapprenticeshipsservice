using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenILoginAUser
{
    private Mock<HomeOrchestrator> _homeOrchestrator;
    private Mock<EmployerAccountsConfiguration> _configuration;
    private HomeController _homeController;    
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private Mock<IUrlActionHelper> _urlActionHelper;

    [SetUp]
    public void Arrange()
    {
        _homeOrchestrator = new Mock<HomeOrchestrator>();
        _configuration = new Mock<EmployerAccountsConfiguration>();          
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        _urlActionHelper = new Mock<IUrlActionHelper>();

        _homeController = new HomeController(
            _homeOrchestrator.Object, 
            _configuration.Object, 
            _flashMessage.Object,
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ILogger<HomeController>>(), null, null,
            _urlActionHelper.Object);
    }

    [Test]
    public void When_GovSignIn_False_ThenTheUserIsRedirectedToIndex()
    {
        //arrange
        _configuration.Object.UseGovSignIn = false;

        //Act
        var actual = _homeController.SignIn();
            
        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectToActionResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual(ControllerConstants.IndexActionName, actualRedirectResult.ActionName);
    }


    [Test]
    public void When_Route_To_PreAuth_ThenTheUserIsRedirectedToIndex()
    {
        //Act
        var actual = _homeController.GovSignIn();

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectToActionResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual("Index", actualRedirectResult.ActionName);
    }
}