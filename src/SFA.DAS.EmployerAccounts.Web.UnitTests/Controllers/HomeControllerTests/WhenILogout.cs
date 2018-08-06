using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Authorization;using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Authenication;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenILogout
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private HomeController _homeController;
        private Mock<EmployerAccountsConfiguration> _configuration;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IAuthenticationService>();
            _configuration = new Mock<EmployerAccountsConfiguration>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _homeController = new HomeController(
                _owinWrapper.Object, null, _configuration.Object, _featureToggle.Object, _userViewTestingService.Object, 
                _flashMessage.Object);
        }
    }
}
