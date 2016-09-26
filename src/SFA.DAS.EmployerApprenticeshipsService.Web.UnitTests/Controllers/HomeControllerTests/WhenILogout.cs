using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenILogout
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private HomeController _homeController;
        private Mock<EmployerApprenticeshipsServiceConfiguration> _configuration;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _configuration = new Mock<EmployerApprenticeshipsServiceConfiguration>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();

            _homeController = new HomeController(
                _owinWrapper.Object, null, _configuration.Object, _featureToggle.Object, _userWhiteList.Object);
        }

        [Test]
        public void ThenTheOwinWrapperSignOutIsCalled()
        {
            //Act
            _homeController.SignOut();

            //Assert
            _owinWrapper.Verify(x=>x.SignOutUser());
        }
    }
}
