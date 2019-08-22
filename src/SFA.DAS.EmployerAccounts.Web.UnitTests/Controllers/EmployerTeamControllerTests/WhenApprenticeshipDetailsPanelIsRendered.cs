using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System;
using System.Globalization;
using System.Web.Mvc;
using Model = SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    public class WhenApprenticeshipDetailsPanelIsRendered
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
        public void ThenTheModelContainsTheExpectedDataForTheApprenticeshipDetails()
        {
            // Arrange
            var firstName = Guid.NewGuid().ToString();
            var lastName = Guid.NewGuid().ToString();
            var providerName = Guid.NewGuid().ToString();
            var courseName = Guid.NewGuid().ToString();
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now.AddDays(2);
            decimal proposedCost = 123.45M;

            var model = new AccountDashboardViewModel
            {
                AccountViewModel = new Model.Account(),
            };

            var testOrganisation = new Model.Organisation();

            var testProvider = new Model.Provider
            {
                Name = providerName
            };

            var testCohort = new Model.Cohort
            {
                IsApproved = true
            };

            var testApprenticeship = new Model.Apprenticeship
            {
                FirstName = firstName,
                LastName = lastName,
                TrainingProvider = testProvider,
                StartDate = startdate,
                EndDate = enddate,
                ProposedCost = proposedCost,
                CourseName = courseName
            };

            testApprenticeship.TrainingProvider = testProvider;
            testCohort.Apprenticeships.Add(testApprenticeship);
            testOrganisation.Cohorts.Add(testCohort);
            model.AccountViewModel.Organisations.Add(testOrganisation);

            //Act
            var result = (_controller.ApprenticeshipDetails(model) as PartialViewResult).Model as ApprenticeDetailsViewModel;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual($"{firstName} {lastName}", result.ApprenticeName);
            Assert.AreEqual(providerName, result.TrainingProviderName);
            Assert.AreEqual(courseName, result.CourseName);
            Assert.AreEqual(startdate.ToString("MMMM yyyy"), result.StartDateText);
            Assert.AreEqual(enddate.ToString("MMMM yyyy"), result.EndDateText);
            Assert.AreEqual($"{proposedCost.ToString("C0", CultureInfo.CreateSpecificCulture("en-GB"))} excluding VAT", result.ProposedCostText);
            Assert.IsTrue(result.IsApproved);
        }
    }    
}
