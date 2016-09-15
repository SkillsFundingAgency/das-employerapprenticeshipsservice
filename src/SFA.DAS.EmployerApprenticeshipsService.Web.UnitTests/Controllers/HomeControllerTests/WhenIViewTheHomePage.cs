using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIViewTheHomePage
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private HomeController _homeController;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private Mock<EmployerApprenticeshipsServiceConfiguration> _configuration;
        private string ExpectedUserId = "123ABC";

        [SetUp]
        public void Arrange()
        {

            _owinWrapper = new Mock<IOwinWrapper>();

            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _homeOrchestrator.Setup(x => x.GetUsers()).ReturnsAsync(new SignInUserViewModel());
            _homeOrchestrator.Setup(x => x.GetUserAccounts(ExpectedUserId)).ReturnsAsync(new OrchestratorResponse<UserAccountsViewModel> {Data = new UserAccountsViewModel()});

            _configuration = new Mock<EmployerApprenticeshipsServiceConfiguration>();

            _homeController = new HomeController(_owinWrapper.Object, _homeOrchestrator.Object, _configuration.Object);
        }

        [Test]
        public async Task ThenTheAccountsAreNotReturnedWhenYouAreNotAuthenticated()
        {
            //Act
            await _homeController.Index();

            //Assert
            _homeOrchestrator.Verify(x=>x.GetUserAccounts(It.IsAny<string>()),Times.Never);
        }

        [Test]
        public async Task ThenTheAccountsAreReturnedForThatUserWhenAuthenticated()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);

            //Act
            await _homeController.Index();

            //Assert
            _homeOrchestrator.Verify(x => x.GetUserAccounts(ExpectedUserId), Times.Once);
        }

        [Test]
        public void ThenTheIndexDoesNotHaveTheAuthorizeAttribute()
        {
            var attributes = typeof(HomeController).GetMethod("Index").GetCustomAttributes(true).ToList();

            foreach (var attribute in attributes)
            {
                var actual = attribute as AuthorizeAttribute;
                Assert.IsNull(actual);
            }
        }

        [Test]
        public async Task ThenTheUnauthenticatedViewIsReturnedWhenNoUserIsLoggedIn()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("");

            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual("ServiceLandingPage", actualViewResult.ViewName);
        }

        [Test]
        public async Task ThenTheModelIsPassedToTheView()
        {
            //Act
            var actual = await _homeController.FakeUserSignIn();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            var actualModel = actualViewResult.Model as SignInUserViewModel;
            Assert.IsNotNull(actualModel);
        }
    }
}
