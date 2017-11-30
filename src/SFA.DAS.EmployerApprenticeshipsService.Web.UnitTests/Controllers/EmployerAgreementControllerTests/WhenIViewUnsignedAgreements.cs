using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAgreementControllerTests
{
    public class WhenIViewUnsignedAgreements
    {
        private EmployerAgreementController _controller;
        private Mock<EmployerAgreementOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private string _hashedAccountId;
        private string _externalUserId;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAgreementOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _hashedAccountId = "ABC123";
            _externalUserId = "23324";

            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(_externalUserId);

            _controller = new EmployerAgreementController(
                _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userViewTestingService.Object,
                _flashMessage.Object);
        }

        [Test]
        public async Task ThenIShouldGoStraightToTheUnsignedAgreementIfThereIsOnlyOne()
        {
            //Arrange
            const string hashedAgreementId = "CCC223";

            _orchestrator.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Data = new EmployerAgreementListViewModel
                    {
                        EmployerAgreements = new List<EmployerAgreementView>
                        {
                            new EmployerAgreementView{ HashedAgreementId = hashedAgreementId, Status = EmployerAgreementStatus.Pending},
                            new EmployerAgreementView{ HashedAgreementId = "JH4545", Status = EmployerAgreementStatus.Signed}
                        }
                    }
                });

            //Act
            var result = await _controller.ViewUnsignedAgreements(_hashedAccountId) as RedirectToRouteResult;

            //Assert
            _owinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.SubClaimKeyName));
            _orchestrator.Verify(x => x.Get(_hashedAccountId, _externalUserId));
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "AboutYourAgreement");
            Assert.AreEqual(result.RouteValues["agreementId"], hashedAgreementId);
        }

        [Test]
        public async Task ThenIShouldSeeAllAgreementsIfIHaveMoreThanASingleUnsignedAgreement()
        {
            //Arrange
            _orchestrator.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Data = new EmployerAgreementListViewModel
                    {
                        EmployerAgreements = new List<EmployerAgreementView>
                        {
                            new EmployerAgreementView{ HashedAgreementId = "GHJ356", Status = EmployerAgreementStatus.Pending},
                            new EmployerAgreementView{ HashedAgreementId = "JH4545", Status = EmployerAgreementStatus.Pending}
                        }
                    }
                });

            //Act
            var result = await _controller.ViewUnsignedAgreements(_hashedAccountId) as RedirectToRouteResult;

            //Assert
            _owinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.SubClaimKeyName));
            _orchestrator.Verify(x => x.Get(_hashedAccountId, _externalUserId));
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
        }
    }
}
