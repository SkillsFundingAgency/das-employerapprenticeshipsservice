using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System.Web.Mvc;
using Model = SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    public class WhenHasSingleClosedVacancy
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> mockAuthenticationService;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IMultiVariantTestingService> mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> mockEmployerTeamOrchestrator;
        private Mock<IPortalClient> mockPortalClient;

        [SetUp]
        public void Arrange()
        {
            mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthorizationService = new Mock<IAuthorizationService>();
            mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            mockPortalClient = new Mock<IPortalClient>();

            _controller = new EmployerTeamController(
                mockAuthenticationService.Object,
                mockAuthorizationService.Object,
                mockMultiVariantTestingService.Object,
                mockCookieStorageService.Object,
                mockEmployerTeamOrchestrator.Object,
                mockPortalClient.Object);
        }

        [Test]
        public void ThenTheVacancyStatusViewIsReturned()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                AccountViewModel = new Model.Account()
            };

            model.AccountViewModel.Vacancies.Add(new Model.Vacancy { Status = Model.VacancyStatus.Closed });

            //Act
            var result = _controller.Row2Panel2(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("VacancyStatus", (result.Model as PanelViewModel<AccountDashboardViewModel>).ViewName);
        }
    }    
}
