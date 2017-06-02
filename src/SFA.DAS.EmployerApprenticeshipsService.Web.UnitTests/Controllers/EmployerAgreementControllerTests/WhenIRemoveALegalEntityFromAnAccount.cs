using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAgreementControllerTests
{
    public class WhenIRemoveALegalEntityFromAnAccount
    {
        private EmployerAgreementController _controller;
        private Mock<EmployerAgreementOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        public const string ExpectedUserId = "AFV456TGF";
        public const string ExpectedHashedAccountId = "FFRE45";
        public const string ExpectedHashedAgreementId = "789UHY";

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAgreementOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);

            _controller = new EmployerAgreementController(
                _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userViewTestingService.Object, _flashMessage.Object);
        }

        [Test]
        public async Task ThenTheOrchestratorIsCalledToGetAccountsToRemove()
        {
            //Act
            await _controller.GetOrganisationsToRemove(ExpectedHashedAccountId);

            //Assert
            _orchestrator.Verify(x=>x.GetLegalAgreementsToRemove(ExpectedHashedAccountId, ExpectedUserId), Times.Once);
        }

        [Test]
        public async Task ThenTheOrchestratorIsCalledToGetTheConfirmRemoveModel()
        {
            //Act
            await _controller.ConfirmRemoveOrganisation(ExpectedHashedAgreementId, ExpectedHashedAccountId);

            //Assert
            _orchestrator.Verify(
                x => x.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAgreementId, ExpectedHashedAccountId, ExpectedUserId),Times.Once);
        }

        [Test]
        public async Task ThenTheFlashMessageIsPopulatedFromTheCookieWhenGettingTheConfirmRemoveAction()
        {
            //Arrange
            _flashMessage.Setup(x => x.Get("sfa-das-employerapprenticeshipsservice-flashmessage")).Returns(new FlashMessageViewModel {Headline = ""});
            _orchestrator.Setup(x => x.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAgreementId, ExpectedHashedAccountId, ExpectedUserId))
                .ReturnsAsync(new OrchestratorResponse<ConfirmLegalAgreementToRemoveViewModel>
                {
                    Data = new ConfirmLegalAgreementToRemoveViewModel()
                });

            //Act
            var actual = await _controller.ConfirmRemoveOrganisation(ExpectedHashedAgreementId, ExpectedHashedAccountId);

            //Assert
            Assert.IsNotNull(actual);
            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
            var actualModel = viewResult.Model as OrchestratorResponse<ConfirmLegalAgreementToRemoveViewModel>;
            Assert.IsNotNull(actualModel);
            Assert.IsNotNull(actualModel.FlashMessage);
        }

        [Test]
        public async Task ThenTheOrchestratorIsCalledToRemoveTheOrg()
        {
            //Arrange
            _orchestrator.Setup(x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), ExpectedUserId))
                .ReturnsAsync(new OrchestratorResponse<bool> { Status = HttpStatusCode.OK, FlashMessage = new FlashMessageViewModel() });

            //Act
            await _controller.RemoveOrganisation(ExpectedHashedAccountId, ExpectedHashedAgreementId, new ConfirmLegalAgreementToRemoveViewModel());

            //Assert
            _orchestrator.Verify(
                x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), ExpectedUserId), Times.Once);
        }

        [TestCase(HttpStatusCode.Accepted, "Index", 0)]
        [TestCase(HttpStatusCode.BadRequest, "ConfirmRemoveOrganisation", 1)]
        [TestCase(HttpStatusCode.OK, "Index", 1)]
        public async Task ThenTheActionRedirectsToTheCorrectViewWhenRemovingTheOrg(HttpStatusCode code, string viewName, int isFlashPopulated)
        {
            //Arrange
            _orchestrator.Setup(x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), ExpectedUserId))
                .ReturnsAsync(new OrchestratorResponse<bool> {Status = code, FlashMessage = new FlashMessageViewModel()});

            //Act
            var actual = await _controller.RemoveOrganisation(ExpectedHashedAccountId, ExpectedHashedAgreementId, new ConfirmLegalAgreementToRemoveViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var redirectResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(viewName,redirectResult.RouteValues["Action"]);
            _flashMessage.Verify(x=>x.Create(It.IsAny<FlashMessageViewModel>(), "sfa-das-employerapprenticeshipsservice-flashmessage",1),Times.Exactly(isFlashPopulated));
        }

    }
    
}
