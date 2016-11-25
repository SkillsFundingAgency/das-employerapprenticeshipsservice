using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountPayeControllerTests
{
    public class WhenIRemoveAPayeScheme
    {
        private Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private EmployerAccountPayeController _controller;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;

        [SetUp]
        public void Arrange()
        {
            _employerAccountPayeOrchestrator = new Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator>();
            _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeScheme>())).ReturnsAsync(new OrchestratorResponse<RemovePayeScheme>());
            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("123abc");
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();

            _controller = new EmployerAccountPayeController(
                _owinWrapper.Object, _employerAccountPayeOrchestrator.Object, _featureToggle.Object, _userWhiteList.Object);
        }

        [Test]
        public async Task ThenTheOrchestratorIsCalledIfYouConfirmToRemoveTheScheme()
        {
            //Act
            var actual = await _controller.RemovePaye("", new RemovePayeScheme { RemoveScheme = 2 });

            //Assert
            _employerAccountPayeOrchestrator.Verify(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeScheme>()), Times.Once);
            Assert.IsNotNull(actual);
            var actualRedirect = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual("Index", actualRedirect.RouteValues["Action"]);
            Assert.AreEqual("EmployerAccountPaye", actualRedirect.RouteValues["Controller"]);
            Assert.IsTrue(_controller.TempData.ContainsKey("successMessage"));
        }

        [Test]
        public async Task ThenTheConfirmRemoveSchemeViewIsReturnedIfThereIsAValidationError()
        {
            //Arrange
            _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeScheme>())).ReturnsAsync(new OrchestratorResponse<RemovePayeScheme> { Status = HttpStatusCode.BadRequest });

            //Act
            var actual = await _controller.RemovePaye("", new RemovePayeScheme());

            //Assert
            Assert.IsNotNull(actual);
            var actualView = actual as ViewResult;
            Assert.IsNotNull(actualView);
            Assert.AreEqual("Remove", actualView.ViewName);
        }
    }
}
