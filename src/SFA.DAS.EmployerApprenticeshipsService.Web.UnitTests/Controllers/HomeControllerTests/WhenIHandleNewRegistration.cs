using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIHandleNewRegistration
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private HomeController _homeController;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private string ExpectedUserId = "123ABC";

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IAuthenticationService>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();

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
