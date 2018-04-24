using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    class WhenIViewNextSteps
    {
        private EmployerTeamController _controller;
        private Mock<EmployerTeamOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            
            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _orchestrator = new Mock<EmployerTeamOrchestrator>(new Mock<IMediator>().Object, Mock.Of<ICurrentDateTime>());

            _controller = new EmployerTeamController(
                _owinWrapper.Object,
                _featureToggle.Object,
                _userViewTestingService.Object,
                _flashMessage.Object,
                _orchestrator.Object);
        }

        [Test]
        public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizard()
        {
            //Arrange
            const string userId = "123";
            const string hashedAccountId = "ABC123";

            _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
            _orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var result = _controller.NextSteps(hashedAccountId).Result as ViewResult;
            var model = result?.Model as OrchestratorResponse<InviteTeamMemberNextStepsViewModel>;

            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Data.UserShownWizard);
            _orchestrator.Verify(x => x.UserShownWizard(userId, hashedAccountId), Times.Once);
        }

        [Test]
        public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizardWhenIMakeAnIncorrectStepSelection()
        {
            //Arrange
            const string userId = "123";
            const string hashedAccountId = "ABC123";

            _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
            _orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(true);

            //Act
            var result = _controller.NextSteps(hashedAccountId).Result as ViewResult;
            var model = result?.Model as OrchestratorResponse<InviteTeamMemberNextStepsViewModel>;

            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Data.UserShownWizard);
            _orchestrator.Verify(x => x.UserShownWizard(userId, hashedAccountId), Times.Once);
        }
    }
}
