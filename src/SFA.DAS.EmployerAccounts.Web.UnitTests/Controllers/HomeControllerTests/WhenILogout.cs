using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenILogout
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<EmployerAccountsConfiguration> _configuration;      
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IAuthenticationService>();
            _configuration = new Mock<EmployerAccountsConfiguration>();       
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            var homeController = new HomeController(
                _owinWrapper.Object, 
                null,               
                _configuration.Object, 
                _userViewTestingService.Object, 
                _flashMessage.Object,
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ILog>());
        }
    }
}
