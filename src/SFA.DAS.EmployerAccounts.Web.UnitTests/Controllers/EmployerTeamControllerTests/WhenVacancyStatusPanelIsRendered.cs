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
using System;
using System.Web.Mvc;
using AutoFixture;
using Model = SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    public class WhenVacancyStatusPanelIsRendered
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> _mockAuthenticationService;
        private Mock<IAuthorizationService> _mockAuthorizationService;
        private Mock<IMultiVariantTestingService> _mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> _mockEmployerTeamOrchestrator;
        private Mock<IPortalClient> _mockPortalClient;

        [SetUp]
        public void Arrange()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            _mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            _mockPortalClient = new Mock<IPortalClient>();

            _controller = new EmployerTeamController(
                _mockAuthenticationService.Object,
                _mockAuthorizationService.Object,
                _mockMultiVariantTestingService.Object,
                _mockCookieStorageService.Object,
                _mockEmployerTeamOrchestrator.Object,
                _mockPortalClient.Object);
        }

        [TestCase(VacancyStatus.Closed, "Closed",  "Manage vacancy")]
        [TestCase(VacancyStatus.Submitted, "Pending review", "Preview vacancy")]
        [TestCase(VacancyStatus.Draft, "Draft", "Edit and submit vacancy")]
        [TestCase(VacancyStatus.Referred, "Rejected", "Edit and re-submit vacancy")]
        [TestCase(VacancyStatus.Live, "Live", "Manage vacancy")]
        public void ThenTheModelContainsTheExpectedDataForTheVacancyStatus(VacancyStatus vacancyStatus, string status, string linkText)
        {
            // Arrange
            var vacancyTitle = Guid.NewGuid().ToString();
            DateTime closingdate = DateTime.Now;
            string manageVacancyLinkUrl = $"http://{Guid.NewGuid().ToString()}";
            long reference = 12345;

            var model = new AccountDashboardViewModel
            {
                AccountViewModel = new Model.Account(),
            };

            var testVacancy = new Model.Vacancy
            {
                Title = vacancyTitle,
                Status = vacancyStatus,
                ClosingDate = closingdate,
                ManageVacancyUrl = manageVacancyLinkUrl,
                Reference = reference
            };

            model.AccountViewModel.Vacancies.Add(testVacancy);

            //Act
            var result = (_controller.VacancyStatus(model) as PartialViewResult).Model as VacancyStatusViewModel;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(vacancyTitle, result.VacancyTitle);
            Assert.AreEqual(closingdate.ToString("d MMMM yyyy"), result.ClosingDateText);
            Assert.AreEqual(manageVacancyLinkUrl, result.ManageVacancyLinkUrl);
            Assert.AreEqual(linkText, result.ManageVacancyLinkText);
            Assert.AreEqual("VAC" + reference, result.Reference);            
            Assert.AreEqual(status, result.Status);
        }

        [TestCase(null, VacancyStatus.Draft, ApplicationMethod.ThroughFindAnApprenticeship, 1)]
        [TestCase(null, VacancyStatus.Draft, ApplicationMethod.ThroughExternalApplicationSite, 1)]
        [TestCase(null, VacancyStatus.Submitted, ApplicationMethod.ThroughFindAnApprenticeship, 1)]
        [TestCase(null, VacancyStatus.Submitted, ApplicationMethod.ThroughExternalApplicationSite, 1)]
        [TestCase(null, VacancyStatus.Referred, ApplicationMethod.ThroughFindAnApprenticeship, 1)]
        [TestCase(null, VacancyStatus.Referred, ApplicationMethod.ThroughExternalApplicationSite, 1)]
        [TestCase("111", VacancyStatus.Live, ApplicationMethod.ThroughFindAnApprenticeship, 111)]
        [TestCase("Advertised by employer", VacancyStatus.Live, ApplicationMethod.ThroughExternalApplicationSite, 111)]
        [TestCase("22", VacancyStatus.Closed, ApplicationMethod.ThroughFindAnApprenticeship, 22)]
        [TestCase("Advertised by employer", VacancyStatus.Closed, ApplicationMethod.ThroughExternalApplicationSite, 22)]
        [TestCase(null, VacancyStatus.Approved, ApplicationMethod.ThroughFindAnApprenticeship, 1)]
        [TestCase(null, VacancyStatus.Approved, ApplicationMethod.ThroughExternalApplicationSite, 1)]
        public void ThenTheModelContainsTheExpectedApplication(string expectedApplications, VacancyStatus vacancyStatus, ApplicationMethod applicationMethod, int numberOfApplications)
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                AccountViewModel = new Account()
            };

            var fixture = new Fixture();
            var vacancy = fixture.Create<Vacancy>();
            vacancy.Status = vacancyStatus;
            vacancy.ApplicationMethod = applicationMethod;
            vacancy.NumberOfApplications = numberOfApplications;
            model.AccountViewModel.Vacancies.Add(vacancy);

            //Act
            var result = _controller.VacancyStatus(model);

            //Assert
            Assert.IsNotNull(result);
            var partialViewResult = result as PartialViewResult;
            Assert.IsNotNull(partialViewResult);
            var vacancyStatusViewModel = partialViewResult.Model as VacancyStatusViewModel;
            Assert.IsNotNull(vacancyStatusViewModel);

            Assert.AreEqual(expectedApplications, vacancyStatusViewModel.Applications);
        }
    }
}
