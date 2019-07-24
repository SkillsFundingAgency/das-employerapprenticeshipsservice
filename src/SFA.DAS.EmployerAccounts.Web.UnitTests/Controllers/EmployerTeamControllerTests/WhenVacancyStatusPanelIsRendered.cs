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
using Model = SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    public class WhenVacancyStatusPanelIsRendered
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

        [TestCase(VacancyStatus.Closed, "Closed",  "Manage vacancy", 10)]
        [TestCase(VacancyStatus.Submitted, "Pending review", "Preview vacancy", null)]
        [TestCase(VacancyStatus.Draft, "Draft", "Edit and submit vacancy", null)]
        [TestCase(VacancyStatus.Referred, "Rejected", "Edit and re-submit vacancy", null)]
        public void ThenTheModelContainsTheExpectedDataForTheVacancyStatus(VacancyStatus vacancyStatus, string status, string linkText, int? numberOfAplications)
        {
            // Arrange
            var trainingTitle = Guid.NewGuid().ToString();
            DateTime closingdate = DateTime.Now;
            string manageVacancyLinkUrl = $"http://{Guid.NewGuid().ToString()}";
            long reference = 12345;

            var model = new AccountDashboardViewModel
            {
                AccountViewModel = new Model.Account(),
            };

            var testVacancy = new Model.Vacancy
            {
                TrainingTitle = trainingTitle,
                Status = vacancyStatus,
                ClosingDate = closingdate,
                ManageVacancyUrl = manageVacancyLinkUrl,
                Reference = reference
            };

            if (numberOfAplications.HasValue)
            {
                testVacancy.NumberOfApplications = numberOfAplications.Value;
            }

            model.AccountViewModel.Vacancies.Add(testVacancy);

            //Act
            var result = (_controller.VacancyStatus(model) as PartialViewResult).Model as VacancyStatusViewModel;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(trainingTitle, result.TrainingTitle);
            Assert.AreEqual(closingdate.ToString("d MMMM yyyy"), result.ClosingDateText);
            Assert.AreEqual(manageVacancyLinkUrl, result.ManageVacancyLinkUrl);
            Assert.AreEqual(linkText, result.ManageVacancyLinkText);
            Assert.AreEqual("VAC" + reference, result.Reference);            
            Assert.AreEqual(status, result.Status);
            if (numberOfAplications.HasValue)
            {
                Assert.AreEqual(numberOfAplications.Value, result.NumberOfApplications);
            }
        }
    }    
}
