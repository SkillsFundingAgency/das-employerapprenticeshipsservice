using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    public class WhenGettingCallToActionViewName
    {
        [Test, RecursiveMoqAutoData]
        public void WhenMultipleReservation_ThenShouldGetCheckFundingViewNameNotDisplayCallToAction(
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();
            panelViewModel.Data.PendingAgreements.Clear();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().BeNullOrEmpty();
        }

        [Test, RecursiveMoqAutoData]
        public void WhenMultipleCohorts_ThenShouldGetCheckFundingViewNameNotDisplayCallToAction(
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation>();
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();
            panelViewModel.Data.PendingAgreements.Clear();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().BeNullOrEmpty();
        }

        [Test, RecursiveMoqAutoData]
        public void WhenMultipleApprenticeshipsWithinASingleCohort_ThenShouldGetCheckFundingViewNameNotDisplayCallToAction(
            CohortViewModel singleCohort,
            List<ApprenticeshipViewModel> apprenticeships,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            apprenticeships.ForEach(a =>
            {
                a.HashedCohortId = singleCohort.HashedCohortId;
                a.CohortId = 123;
            });

            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel> { singleCohort };
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = apprenticeships;
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation>();
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();
            panelViewModel.Data.PendingAgreements.Clear();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().BeNullOrEmpty();
        }

        [Test, RecursiveMoqAutoData]
        public void WhenMultipleVacancies_ThenShouldGetCheckFundingViewNameNotDisplayCallToAction(
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation>();
            panelViewModel.Data.PendingAgreements.Clear();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().BeNullOrEmpty();
        }

        [Test, RecursiveMoqAutoData]
        public void WhenPendingAgreementExists_ThenShouldGetSignAgreementCallToAction(
           PendingAgreementsViewModel pendingAgreement,
           [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
           EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.PendingAgreements = new List<PendingAgreementsViewModel> { pendingAgreement };

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().Be("SignAgreement");
        }

        [Test, RecursiveMoqAutoData]
        public void WhenNoData_ThenDoNotSetCallToActionViewName(
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.CallToActionViewModel = null;
            panelViewModel.Data.PendingAgreements.Clear();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().BeNullOrEmpty();
        }

        [Test, RecursiveMoqAutoData]
        public void WhenLevy_IgnoreCallToActionRules(
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.CallToActionViewModel = null;
            panelViewModel.Data.PendingAgreements.Clear();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().BeNullOrEmpty();
        }

        [Test, RecursiveMoqAutoData]
        public void WhenNoSetup_ThenShouldGetCheckFundingViewName(
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation>();
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();
            panelViewModel.Data.PendingAgreements.Clear();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().Be("CheckFunding");
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleReservation_ThenShouldGetContinueSetupForSingleReservationViewName(
            Reservation singleReservation,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();
            panelViewModel.Data.PendingAgreements.Clear();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ViewName.Should().Be("ContinueSetupForSingleReservation");
        }
    }
}
