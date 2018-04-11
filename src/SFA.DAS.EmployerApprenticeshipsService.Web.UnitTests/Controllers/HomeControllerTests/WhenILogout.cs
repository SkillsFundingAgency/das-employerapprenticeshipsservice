using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenILogout
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private HomeController _homeController;
        private Mock<EmployerApprenticeshipsServiceConfiguration> _configuration;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IAuthenticationService>();
            _configuration = new Mock<EmployerApprenticeshipsServiceConfiguration>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _homeController = new HomeController(
                _owinWrapper.Object, null, _configuration.Object, _featureToggle.Object, _userViewTestingService.Object, 
                _flashMessage.Object);
        }
    }
}
