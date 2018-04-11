using System.Web.Mvc;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationControllerTests
{
    class WhenIViewNextSteps
    {
        private OrganisationController _controller;
        private Mock<OrganisationOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<IMapper> _mapper;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<OrganisationOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _mapper = new Mock<IMapper>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            
            _logger = new Mock<ILog>();

            _controller = new OrganisationController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _featureToggle.Object,
                _userViewTestingService.Object,
                _mapper.Object,
                _logger.Object,
                _flashMessage.Object);
        }

        [Test]
        public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizard()
        {
            //Arrange
            const string userId = "123";
            const string hashedAccountId = "ABC123";

            _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
            _orchestrator.Setup(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
                {
                    Data = new OrganisationAddedNextStepsViewModel { ShowWizard = true}
                });

            //Act
            var result = _controller.OrganisationAddedNextSteps("test", hashedAccountId).Result as ViewResult;
            var model = result?.Model as OrchestratorResponse<OrganisationAddedNextStepsViewModel>;

            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Data.ShowWizard);
            _orchestrator.Verify(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), userId, hashedAccountId), Times.Once);
        }

        [Test]
        public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizardWhenSearching()
        {
            //Arrange
            const string userId = "123";
            const string hashedAccountId = "ABC123";
            const string hashedAgreementId = "DEF456";

            _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
            _orchestrator.Setup(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
                {
                    Data = new OrganisationAddedNextStepsViewModel { ShowWizard = true }
                });

            //Act
            var result = _controller.OrganisationAddedNextStepsSearch("test", hashedAccountId, hashedAgreementId).Result as ViewResult;
            var model = result?.Model as OrchestratorResponse<OrganisationAddedNextStepsViewModel>;

            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Data.ShowWizard);
            _orchestrator.Verify(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), userId, hashedAccountId, hashedAgreementId), Times.Once);
        }

        [Test]
        public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizardWhenIMakeAnIncorrectStepSelection()
        {
            //Arrange
            const string userId = "123";
            const string hashedAccountId = "ABC123";
            const string hashedAgreementId = "DEF456";

            _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
            _orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(true);

            //Act
            var result = _controller.GoToNextStep("Not A Step", hashedAccountId, hashedAgreementId, "test").Result as ViewResult;
            var model = result?.Model as OrchestratorResponse<OrganisationAddedNextStepsViewModel>;

            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Data.ShowWizard);
            _orchestrator.Verify(x => x.UserShownWizard(userId, hashedAccountId), Times.Once);
        }
    }
}
