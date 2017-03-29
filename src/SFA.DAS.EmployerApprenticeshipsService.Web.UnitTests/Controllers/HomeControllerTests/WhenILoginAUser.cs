using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenILoginAUser
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private Mock<EmployerApprenticeshipsServiceConfiguration> _configuration;
        private HomeController _homeController;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserViewTestingService> _userViewTestingService;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _configuration = new Mock<EmployerApprenticeshipsServiceConfiguration>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IUserViewTestingService>();

            _homeController = new HomeController(
                _owinWrapper.Object, _homeOrchestrator.Object, _configuration.Object, _featureToggle.Object, _userViewTestingService.Object);
        }

        [Test]
        public void ThenTheUserIsRedirectedToTheIndex()
        {
            //Act
            var actual = _homeController.SignIn();
            
            //Assert
            Assert.IsNotNull(actual);
            var actualRedirectResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirectResult);
            Assert.AreEqual("Index",actualRedirectResult.RouteValues["Action"]);

        }
    }
}
