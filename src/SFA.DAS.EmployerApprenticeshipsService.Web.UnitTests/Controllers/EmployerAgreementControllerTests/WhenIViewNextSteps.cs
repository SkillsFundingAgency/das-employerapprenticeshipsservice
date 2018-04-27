using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAgreementControllerTests
{
    class WhenIViewNextSteps
    {
        private EmployerAgreementController _controller;
        private Mock<EmployerAgreementOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAgreementOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _controller = new EmployerAgreementController(_owinWrapper.Object, _orchestrator.Object, 
                _featureToggle.Object, _userViewTestingService.Object,_flashMessage.Object);
        }

        [Test]
        public void ShouldShowIfUserCanSeeWizardWhenViewingNextSteps()
        {
            //Arrange
            const string userId = "123";
            const string hashedAccountId = "ABC123";

            _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
            _orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var result = _controller.NextSteps(hashedAccountId).Result as ViewResult;
            var model = result?.Model as OrchestratorResponse<EmployerAgreementNextStepsViewModel>;

            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Data.UserShownWizard);
            _orchestrator.Verify(x => x.UserShownWizard(userId, hashedAccountId), Times.Once);
        }

        [Test]
        public void ShouldShowIfUserCanSeeWizardWhenSelectingIncorrectChoiceForNextSteps()
        {
            //Arrange
            const string userId = "123";
            const string hashedAccountId = "ABC123";

            _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
            _orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var result = _controller.NextSteps(0, hashedAccountId).Result as ViewResult;
            var model = result?.Model as OrchestratorResponse<EmployerAgreementNextStepsViewModel>;

            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Data.UserShownWizard);
            _orchestrator.Verify(x => x.UserShownWizard(userId, hashedAccountId), Times.Once);
        }

    }
}
