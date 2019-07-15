using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests
{
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
                _owinWrapper.Object, 
                _homeOrchestrator.Object,            
                _configuration,
                Mock.Of<IMultiVariantTestingService>(), 
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>());
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
}
