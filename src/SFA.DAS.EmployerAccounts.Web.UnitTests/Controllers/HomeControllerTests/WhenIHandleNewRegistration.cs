using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authenication;
using SFA.DAS.EmployerAccounts.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIHandleNewRegistration
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private HomeController _homeController;
        private EmployerAccountsConfiguration _configuration;
        private string ExpectedUserId = "123ABC";

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IAuthenticationService>();
            _configuration = new EmployerAccountsConfiguration();

            _homeController = new HomeController(
                _owinWrapper.Object, Mock.Of<HomeOrchestrator>(), _configuration, Mock.Of<IAuthorizationService>(),
                Mock.Of<IMultiVariantTestingService>(), Mock.Of<ICookieStorageService<FlashMessageViewModel>>());
        }


        [Test]
        public async Task ThenTheClaimsAreRefreshedForThatUserWhenAuthenticated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);

            //Act
            await _homeController.HandleNewRegistration();

            //Assert
            _owinWrapper.Verify(x => x.UpdateClaims(), Times.Once);
        }
    }
}
