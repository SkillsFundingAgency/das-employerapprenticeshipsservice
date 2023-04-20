using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenILoginAUser
{
    private Mock<HomeOrchestrator> _homeOrchestrator;
    private Mock<EmployerAccountsConfiguration> _configuration;
    private HomeController _homeController;    
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private Mock<GovSignInIdentityConfiguration> _govSignInIdentityConfiguration;

    [SetUp]
    public void Arrange()
    {
        _homeOrchestrator = new Mock<HomeOrchestrator>();
        _configuration = new Mock<EmployerAccountsConfiguration>();          
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        _govSignInIdentityConfiguration = new Mock<GovSignInIdentityConfiguration>();

        _homeController = new HomeController(
            _homeOrchestrator.Object, 
            _configuration.Object, 
            _flashMessage.Object,
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ILogger<HomeController>>(), null, null);
    }

    [Test]
    public void When_GovSignIn_False_ThenTheUserIsRedirectedToPreAuth()
    {
        //arrange
        _configuration.Object.UseGovSignIn = false;

        //Act
        var actual = _homeController.SignIn();
            
        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectToActionResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual(ControllerConstants.PreAuthActionName, actualRedirectResult.ActionName);
    }

    [Test, MoqAutoData]
    public void When_GovSignIn_True_ThenTheUserIsRedirectedToTheGov(string baseUrl)
    {
        //arrange
        _configuration.Object.UseGovSignIn = true;
        _govSignInIdentityConfiguration.Object.BaseUrl = baseUrl;
        _govSignInIdentityConfiguration.Object.SignInLink = "Sign-in";
        _configuration.Object.GovSignInIdentity = _govSignInIdentityConfiguration.Object;

        //Act
        var actual = _homeController.SignIn();

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual($"{_configuration.Object.GovSignInIdentity.BaseUrl}/{_configuration.Object.GovSignInIdentity.SignInLink}", actualRedirectResult.Url);
    }

    [Test]
    public void When_Route_To_PreAuth_ThenTheUserIsRedirectedToIndex()
    {
        //Act
        var actual = _homeController.PreAuth();

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectToActionResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual("Index", actualRedirectResult.ActionName);
    }
}