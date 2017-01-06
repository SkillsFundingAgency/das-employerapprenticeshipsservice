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
    public class WhenIModifyMyUserAccount : ControllerTestBase
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhitelist;
        private HomeController _homeController;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IOwinWrapper>();
            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhitelist = new Mock<IUserWhiteList>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _homeController = new HomeController(_owinWrapper.Object, _homeOrchestrator.Object, _configuration, _featureToggle.Object, _userWhitelist.Object)
            {
                ControllerContext = _controllerContext.Object
            };
        }

        [Test]
        public void ThenWhenIChangedMyPasswordTheViewDataIsPopulatedWithPasswordChangedInformation()
        {
            //Act
            _homeController.HandlePasswordChanged();

            //Assert
            Assert.AreEqual("/user-changed-password", _homeController.TempData["virtualPageUrl"]);
            Assert.AreEqual("User Action - Changed Password", _homeController.TempData["virtualPageTitle"]);
        }

        [Test]
        public void ThenThePasswordChangedActionCreatsARedirectToRouteResultToTheIndex()
        {
            //Act
            var actual = _homeController.HandlePasswordChanged();

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<RedirectToRouteResult>(actual);
            var actualRedirect = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual("Index", actualRedirect.RouteValues["action"]);
        }

        [Test]
        public void ThenWhenICreateAnAccountTheViewDataIsPopulatedWithTheAccountCreatedInformation()
        {
            //Act
            _homeController.HandleNewRegistration();

            //Assert
            Assert.AreEqual("/user-created-account", _homeController.TempData["virtualPageUrl"]);
            Assert.AreEqual("User Action - Created Account", _homeController.TempData["virtualPageTitle"]);
        }

        [Test]
        public void ThenIfTheHandleEmailChangedIsCancelledAndTheQueryParamIsSetTheTempDataValuesAreNotSet()
        {
            
            //Act
            _homeController.HandleEmailChanged(true);

            //Assert
            Assert.IsFalse(_homeController.TempData.ContainsKey("successMessage"));
        }


        [Test]
        public void ThenIfTheHandlePasswordChangedIsCancelledAndTheQueryParamIsSetTheTempDataValuesAreNotSet()
        {

            //Act
            _homeController.HandlePasswordChanged(true);

            //Assert
            Assert.IsFalse(_homeController.TempData.ContainsKey("successMessage"));
        }

        [Test]
        public void ThenTheAccountCreatedActionCreatesARedirectToRouteResultToTheIndex()
        {
            //Act
            var actual = _homeController.HandleNewRegistration();

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<RedirectToRouteResult>(actual);
            var actualRedirect = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual("Index", actualRedirect.RouteValues["action"]);
        }
    }
}
