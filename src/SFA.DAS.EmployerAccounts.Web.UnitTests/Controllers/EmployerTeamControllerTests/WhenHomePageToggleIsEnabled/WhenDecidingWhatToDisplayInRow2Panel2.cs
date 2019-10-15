using System.Linq;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.FeatureToggles;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests.WhenHomePageToggleIsEnabled
{
    // not a real 'when'!
    public class WhenDecidingWhatToDisplayInRow2Panel2
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> _mockAuthenticationService;
        private Mock<IMultiVariantTestingService> _mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> _mockEmployerTeamOrchestrator;
        private Mock<IPortalClient> _mockPortalClient;
        private Mock<IBooleanToggleValueProvider> _mockFeatureToggleProvider;

        [SetUp]
        public void Arrange()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            _mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            _mockPortalClient = new Mock<IPortalClient>();
            _mockFeatureToggleProvider = new Mock<IBooleanToggleValueProvider>();

            FeatureToggles.Features.BooleanToggleValueProvider = _mockFeatureToggleProvider.Object;
            _mockFeatureToggleProvider.Setup(m => m.EvaluateBooleanToggleValue(It.IsAny<IFeatureToggle>())).Returns(true);

            _controller = new EmployerTeamController(
                _mockAuthenticationService.Object,
                _mockMultiVariantTestingService.Object,
                _mockCookieStorageService.Object,
                _mockEmployerTeamOrchestrator.Object,
                _mockPortalClient.Object, Mock.Of<IAuthorizationService>());
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
        public void AndAccountHasPayeSchemeAndNoAccountDocumentThenCreateVacancyPanelIsSelected()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
            };

            //Act
            var result = _controller.Row2Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<PanelViewModel<AccountDashboardViewModel>>(result.Model);
            var resultModel = result.Model as PanelViewModel<AccountDashboardViewModel>;
            Assert.AreEqual("CreateVacancy", resultModel.ViewName);
        }

        [TestCase("MultipleVacancies", false, 0)]
        [TestCase("CreateVacancy", true, 0)]
        [TestCase("VacancyStatus", true, 1)]
        [TestCase("MultipleVacancies", true, 2)]
        public void AndAccountHasPayeSchemeThenCorrectVacancyPanelIsSelected(string expectedViewName, bool vacanciesRetrieved, int numVacancies)
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
                AccountViewModel = new Account
                {
                    VacanciesRetrieved = vacanciesRetrieved,
                    Vacancies = Enumerable.Repeat(new Vacancy(), numVacancies).ToList()
                }
            };

            //Act
            var result = _controller.Row2Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<PanelViewModel<AccountDashboardViewModel>>(result.Model);
            var resultModel = result.Model as PanelViewModel<AccountDashboardViewModel>;
            Assert.AreEqual(expectedViewName, resultModel.ViewName);
        }
    }
}
