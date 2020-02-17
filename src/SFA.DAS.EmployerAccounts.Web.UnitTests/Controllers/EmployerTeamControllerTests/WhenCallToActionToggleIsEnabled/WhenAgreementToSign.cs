using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using Model = SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests.WhenCallToActionToggleIsEnabled
{
    public class WhenAgreementToSign
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> mockAuthenticationService;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IMultiVariantTestingService> mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> mockEmployerTeamOrchestrator;
        private Mock<CallToActionOrchestrator> mockCallToActionOrchestrator;
        private Mock<IPortalClient> mockPortalClient;

        [SetUp]
        public void Arrange()
        {
            mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthorizationService = new Mock<IAuthorizationService>();
            mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            mockCallToActionOrchestrator = new Mock<CallToActionOrchestrator>();
            mockPortalClient = new Mock<IPortalClient>();
            mockAuthorizationService.Setup(m => m.IsAuthorized("EmployerFeature.HomePage")).Returns(false);
            mockAuthorizationService.Setup(m => m.IsAuthorized("EmployerFeature.CallToAction")).Returns(true);

            _controller = new EmployerTeamController(
                mockAuthenticationService.Object,
                mockMultiVariantTestingService.Object,
                mockCookieStorageService.Object,
                mockEmployerTeamOrchestrator.Object,
                mockCallToActionOrchestrator.Object,
                mockPortalClient.Object,
                mockAuthorizationService.Object);
        }

        [Test]
        public void ThenTheSignAgreementViewIsReturnedAtRow1Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel();
            model.PayeSchemeCount = 1;
            model.CallToActionViewModel = new CallToActionViewModel
            {
                AgreementsToSign = true
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SignAgreement", (result.Model as dynamic).ViewName);
        }
    }
}
