using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountPayeControllerTests
{
    public class WhenIRemoveAPayeScheme
    {
        private Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private EmployerAccountPayeController _controller;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _employerAccountPayeOrchestrator = new Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator>();
            _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeSchemeViewModel>())).ReturnsAsync(new OrchestratorResponse<RemovePayeSchemeViewModel>());
            _owinWrapper = new Mock<IAuthenticationService>();
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("123abc");
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _controller = new EmployerAccountPayeController(
                _owinWrapper.Object, 
                _employerAccountPayeOrchestrator.Object,
                _userViewTestingService.Object, 
                _flashMessage.Object);
        }

        [Test]
        public async Task ThenTheOrchestratorIsCalledIfYouConfirmToRemoveTheScheme()
        {
            //Act
            var actual = await _controller.RemovePaye("", new RemovePayeSchemeViewModel { RemoveScheme = 2 });

            //Assert
            _employerAccountPayeOrchestrator.Verify(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeSchemeViewModel>()), Times.Once);
            Assert.IsNotNull(actual);
            var actualRedirect = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual("Index", actualRedirect.RouteValues["Action"]);
            Assert.AreEqual("EmployerAccountPaye", actualRedirect.RouteValues["Controller"]);
            _flashMessage.Verify(x => x.Create(It.Is<FlashMessageViewModel>(c => c.HiddenFlashMessageInformation.Equals("page-paye-scheme-deleted")), It.IsAny<string>(), 1));
        }

        [Test]
        public async Task ThenTheConfirmRemoveSchemeViewIsReturnedIfThereIsAValidationError()
        {
            //Arrange
            _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeSchemeViewModel>())).ReturnsAsync(new OrchestratorResponse<RemovePayeSchemeViewModel> { Status = HttpStatusCode.BadRequest });

            //Act
            var actual = await _controller.RemovePaye("", new RemovePayeSchemeViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var actualView = actual as Microsoft.AspNetCore.Mvc.ViewResult;
            Assert.IsNotNull(actualView);
            Assert.AreEqual("Remove", actualView.ViewName);
        }
    }
}
