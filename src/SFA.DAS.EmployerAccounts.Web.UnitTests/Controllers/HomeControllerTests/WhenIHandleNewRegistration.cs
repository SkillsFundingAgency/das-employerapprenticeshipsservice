using Microsoft.Extensions.Logging;
using SFA.DAS.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenIHandleNewRegistration
{
    private Mock<IAuthenticationService> _owinWrapper;
    private HomeController _homeController;
    private EmployerAccountsConfiguration _configuration;
    private Mock<HomeOrchestrator> _homeOrchestrator;  

    [SetUp]
    public void Arrange()
    {
        _owinWrapper = new Mock<IAuthenticationService>();
        _configuration = new EmployerAccountsConfiguration();
        _homeOrchestrator = new Mock<HomeOrchestrator>();

        _homeController = new HomeController(
            _homeOrchestrator.Object,            
            _configuration,
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ILogger<HomeController>>(),
            Mock.Of<IMultiVariantTestingService>());
    }

    [Test]
    public async Task ThenTheClaimsAreRefreshedForThatUserWhenAuthenticated()
    {
        //Arrange
        var expectedEmail = "test@test.com";
        var expectedId = "123456";
        var expectedFirstName = "Test";
        var expectedLastName = "tester";
        _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(expectedId);
        _owinWrapper.Setup(x => x.GetClaimValue("email")).Returns(expectedEmail);
        _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.GivenName)).Returns(expectedFirstName);
        _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.FamilyName)).Returns(expectedLastName);

        //Act
        await _homeController.HandleNewRegistration();

        //Assert
        _owinWrapper.Verify(x => x.UpdateClaims(), Times.Once);
    }
}