using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
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
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private string _hashedAccountId;
        private string _externalUserId;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAgreementOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
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
                        EmployerAgreementsData = new GetAccountEmployerAgreementsResponse {
                            EmployerAgreements = new List<EmployerAgreementStatusView>
                                {
                                    new EmployerAgreementStatusView{ Pending = new PendingEmployerAgreementDetails { HashedAgreementId = hashedAgreementId, Id = 123}},
                                    new EmployerAgreementStatusView{ Signed = new SignedEmployerAgreementDetails { HashedAgreementId = "JH4545", Id = null}}
                                }
                        }
                    }
                });

            //Act
            var result = await _controller.ViewUnsignedAgreements(_hashedAccountId) as RedirectToRouteResult;

            //Assert
            _owinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));
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
                        EmployerAgreementsData =
                            new GetAccountEmployerAgreementsResponse { 
                                EmployerAgreements = new List<EmployerAgreementStatusView>
                                    {
                                        new EmployerAgreementStatusView{ Pending = new PendingEmployerAgreementDetails { HashedAgreementId = "GHJ356" }},
                                        new EmployerAgreementStatusView{ Pending = new PendingEmployerAgreementDetails { HashedAgreementId = "JH4545" }}
                                    }
                            }
                    }
                });

            //Act
            var result = await _controller.ViewUnsignedAgreements(_hashedAccountId) as RedirectToRouteResult;

            //Assert
            _owinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));
            _orchestrator.Verify(x => x.Get(_hashedAccountId, _externalUserId));
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
        }
    }
}
