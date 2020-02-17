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

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests.WhenHomePageToggleIsEnabled
{
    public class WhenFundsToReserve
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> mockAuthenticationService;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IMultiVariantTestingService> mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> mockEmployerTeamOrchestrator;
        private Mock<Row1Panel1Orchestrator> mockRow1Panel1Orchestrator;
        private Mock<IPortalClient> mockPortalClient;

        [SetUp]
        public void Arrange()
        {
            mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthorizationService = new Mock<IAuthorizationService>();
            mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            mockRow1Panel1Orchestrator = new Mock<Row1Panel1Orchestrator>();
            mockPortalClient = new Mock<IPortalClient>();

            mockAuthorizationService.Setup(m => m.IsAuthorized("EmployerFeature.HomePage")).Returns(false);
            mockAuthorizationService.Setup(m => m.IsAuthorized("EmployerFeature.CallToAction")).Returns(false);

            _controller = new EmployerTeamController(
                mockAuthenticationService.Object,
                mockMultiVariantTestingService.Object,
                mockCookieStorageService.Object,
                mockEmployerTeamOrchestrator.Object,
                mockRow1Panel1Orchestrator.Object,
                mockPortalClient.Object,
                mockAuthorizationService.Object);
        }

        [Test]
        public void ThenForNonLevyTheCheckFundingViewIsReturnedAtRow1Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel();
            model.PayeSchemeCount = 1;
            model.CallToActionViewModel = new CallToActionViewModel
            {
                AgreementsToSign = false
            };
            
            model.ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.NonLevy;
            
            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Empty", (result.Model as dynamic).ViewName);
        }

        [Test]
        public void ThenForLevyTheEmptyViewIsReturnedAtRow1Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel();
            model.PayeSchemeCount = 1;
            model.CallToActionViewModel = new CallToActionViewModel
            {
                AgreementsToSign = false
            };
            
            model.ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.Levy;
            
            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Empty", (result.Model as dynamic).ViewName);
        }

        [Test]
        public void ThenTheTasksViewIsReturnedAtRow1Panel2()
        {
            // Arrange
            var model = new AccountDashboardViewModel();
            model.PayeSchemeCount = 1;
            model.CallToActionViewModel = new CallToActionViewModel
            {
                AgreementsToSign = false
            };
            
            //Act
            var result = _controller.Row1Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Tasks", (result.Model as dynamic).ViewName);
        }

        [Test]
        public void ThenTheDashboardViewIsReturnedAtRow2Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel();
            model.PayeSchemeCount = 1;
            model.CallToActionViewModel = new CallToActionViewModel
            {
                AgreementsToSign = false
            };

            //Act
            var result = _controller.Row2Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Dashboard", (result.Model as dynamic).ViewName);
        }

        [Test]
        public void ThenTheEmptyViewIsReturnedAtRow2Panel2()
        {
            // Arrange
            var model = new AccountDashboardViewModel();
            model.PayeSchemeCount = 1;
            model.CallToActionViewModel = new CallToActionViewModel
            {
                AgreementsToSign = false
            };

            //Act
            var result = _controller.Row2Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Empty", (result.Model as dynamic).ViewName);
        }
    }
}
