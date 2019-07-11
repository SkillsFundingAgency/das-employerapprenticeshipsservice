using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    // not a real 'when'!
    public class WhenDecidingWhatToDisplayInRow2Panel2
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> _mockAuthenticationService;
        private Mock<IAuthorizationService> _mockAuthorizationService;
        private Mock<IMultiVariantTestingService> _mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> _mockEmployerTeamOrchestrator;
        private Mock<IPortalClient> _mockPortalClient;
        private Mock<IHashingService> _mockHashingService;

        [SetUp]
        public void Arrange()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            _mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            _mockPortalClient = new Mock<IPortalClient>();
            _mockHashingService = new Mock<IHashingService>();

            _controller = new EmployerTeamController(
                _mockAuthenticationService.Object,
                _mockAuthorizationService.Object,
                _mockMultiVariantTestingService.Object,
                _mockCookieStorageService.Object,
                _mockEmployerTeamOrchestrator.Object,
                _mockPortalClient.Object,
                _mockHashingService.Object);
        }

        [Test]
        public void AndAccountHasNoPayeSchemeThenPrePayeRecruitmentPanelIsSelected()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 0
            };

            //Act
            var result = _controller.Row2Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<PanelViewModel<AccountDashboardViewModel>>(result.Model);
            var resultModel = result.Model as PanelViewModel<AccountDashboardViewModel>;
            Assert.AreEqual("PrePayeRecruitment", resultModel.ViewName);
        }

        [Test]
        public void AndAccountHasPayeSchemeAndHasVacancyInfoIsUnavailableThenVacancyServiceDownPanelIsSelected()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
                AccountViewModel = new Account()
            };

            //Act
            var result = _controller.Row2Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<PanelViewModel<AccountDashboardViewModel>>(result.Model);
            var resultModel = result.Model as PanelViewModel<AccountDashboardViewModel>;
            Assert.AreEqual("VacancyServiceDown", resultModel.ViewName);
        }

        [Test]
        public void AndAccountHasPayeSchemeAndHasNoVacancyThenCreateVacancyPanelIsSelected()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
                AccountViewModel = new Account
                {
                    VacancyCardinality = Cardinality.None
                }
            };

            //Act
            var result = _controller.Row2Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<PanelViewModel<AccountDashboardViewModel>>(result.Model);
            var resultModel = result.Model as PanelViewModel<AccountDashboardViewModel>;
            Assert.AreEqual("CreateVacancy", resultModel.ViewName);
        }

        [Test]
        public void AndAccountHasPayeSchemeAndHasMoreThanOneVacancyThenMultipleVacanciesPanelIsSelected()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
                AccountViewModel = new Account
                {
                    VacancyCardinality = Cardinality.Many
                }
            };

            //Act
            var result = _controller.Row2Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<PanelViewModel<AccountDashboardViewModel>>(result.Model);
            var resultModel = result.Model as PanelViewModel<AccountDashboardViewModel>;
            Assert.AreEqual("MultipleVacancies", resultModel.ViewName);
        }
    }
}
