using FluentAssertions;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Web.ViewComponents;
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

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.Empty);
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

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.Empty);
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

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.Empty);
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

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.Empty);
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
            panelViewModel.ComponentName.Should().Be(ComponentConstants.SignAgreement);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenNoData_ThenDoNotSetCallToActionViewName(
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.CallToActionViewModel = null;

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.Empty);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenLevy_IgnoreCallToActionRules(
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            panelViewModel.Data.ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.Levy;

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.Empty);
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

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.CheckFunding);
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

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.ContinueSetupForSingleReservation);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleReservationAndApprenticeship_ThenShouldGetSingleApprenticeshipContinueSetupViewName(
            CohortViewModel singleCohort,
            Reservation singleReservation,
            ApprenticeshipViewModel singleApprenticeship,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleCohort.Apprenticeships = new List<ApprenticeshipViewModel> { singleApprenticeship };
            singleCohort.CohortStatus = CohortStatus.Draft;
            singleApprenticeship.ApprenticeshipStatus = ApprenticeshipStatus.Draft;
            singleApprenticeship.NumberOfDraftApprentices = 1;
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel> { singleCohort };
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.SingleApprenticeshipContinueSetup);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleReservationAndApprenticeship_ThenShouldGetSingleApprenticeshipWithTrainingProviderViewName(
            CohortViewModel singleCohort,
            Reservation singleReservation,
            ApprenticeshipViewModel singleApprenticeship,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleCohort.Apprenticeships = new List<ApprenticeshipViewModel> { singleApprenticeship };
            singleCohort.CohortStatus = CohortStatus.WithTrainingProvider;
            singleApprenticeship.ApprenticeshipStatus = ApprenticeshipStatus.Draft;
            singleApprenticeship.NumberOfDraftApprentices = 1;
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel> { singleCohort };
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.SingleApprenticeshipWithTrainingProvider);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleReservationAndApprenticeship_ThenShouldGetSingleApprenticeshipReadyForReviewViewName(
            CohortViewModel singleCohort,
            Reservation singleReservation,
            ApprenticeshipViewModel singleApprenticeship,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleCohort.Apprenticeships = new List<ApprenticeshipViewModel> { singleApprenticeship };
            singleCohort.CohortStatus = CohortStatus.Review;
            singleApprenticeship.ApprenticeshipStatus = ApprenticeshipStatus.Draft;
            singleApprenticeship.NumberOfDraftApprentices = 1;
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel> { singleCohort };
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.SingleApprenticeshipReadyForReview);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenNoReservationAndCohortWithMultipleDraftApprenticeships_ThenShouldNotSetViewName(
           CohortViewModel singleCohort,
           Reservation singleReservation,
           List<ApprenticeshipViewModel> apprenticeships,
           [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
           EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleCohort.Apprenticeships = new List<ApprenticeshipViewModel>();
            singleCohort.CohortStatus = CohortStatus.WithTrainingProvider;
            apprenticeships.ForEach(app => app.ApprenticeshipStatus = ApprenticeshipStatus.Draft);
            singleCohort.NumberOfDraftApprentices = 2;
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel> { singleCohort };
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation>();
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.Empty);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleReservationAndCohortWithNoApprenticeship_ThenShouldGetSingleApprenticeshipContinueWithProviderViewName(
           CohortViewModel singleCohort,
           Reservation singleReservation,
           ApprenticeshipViewModel singleApprenticeship,
           [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
           EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleCohort.Apprenticeships = new List<ApprenticeshipViewModel>();
            singleCohort.CohortStatus = CohortStatus.WithTrainingProvider;
            singleApprenticeship.ApprenticeshipStatus = ApprenticeshipStatus.Draft;
            singleApprenticeship.NumberOfDraftApprentices = 1;
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel> { singleCohort };
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.ContinueSetupForSingleReservation);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleReservationAndCohortWithApprovedApprenticeship_ThenShouldGetSingleApprenticeshipApprovedViewName(
           CohortViewModel singleCohort,
           Reservation singleReservation,
           ApprenticeshipViewModel singleApprenticeship,
           [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
           EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleCohort.Apprenticeships = new List<ApprenticeshipViewModel>();
            singleCohort.CohortStatus = CohortStatus.Approved;
            singleApprenticeship.ApprenticeshipStatus = ApprenticeshipStatus.Approved;
            singleApprenticeship.NumberOfDraftApprentices = 1;
            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel> { singleCohort };
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel> { singleApprenticeship };
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel();

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.SingleApprenticeshipApproved);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleAdvert_ThenShouldGetVacancyDraftName(
            VacancyViewModel singleVacancy,
            Reservation singleReservation,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleVacancy.Status = VacancyStatus.Draft;

            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel {  Vacancies = new List<VacancyViewModel> { singleVacancy } };

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.VacancyDraft);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleAdvert_ThenShouldGetVacancyPendingReviewName(
            VacancyViewModel singleVacancy,
            Reservation singleReservation,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleVacancy.Status = VacancyStatus.Submitted;

            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel { Vacancies = new List<VacancyViewModel> { singleVacancy } };

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.VacancyPendingReview);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleAdvert_ThenShouldGetVacancyLiveName(
            VacancyViewModel singleVacancy,
            Reservation singleReservation,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleVacancy.Status = VacancyStatus.Live;

            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel { Vacancies = new List<VacancyViewModel> { singleVacancy } };

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.VacancyLive);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleAdvert_ThenShouldGetVacancyRejectedName(
            VacancyViewModel singleVacancy,
            Reservation singleReservation,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleVacancy.Status = VacancyStatus.Referred;

            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel { Vacancies = new List<VacancyViewModel> { singleVacancy } };

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.VacancyRejected);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenSingleAdvert_ThenShouldGetVacancyClosedName(
            VacancyViewModel singleVacancy,
            Reservation singleReservation,
            [NonLevyPanelView] PanelViewModel<AccountDashboardViewModel> panelViewModel,
            EmployerTeamOrchestrator sut)
        {
            // Arrange
            singleVacancy.Status = VacancyStatus.Closed;

            panelViewModel.Data.CallToActionViewModel.Cohorts = new List<CohortViewModel>();
            panelViewModel.Data.CallToActionViewModel.Apprenticeships = new List<ApprenticeshipViewModel>();
            panelViewModel.Data.CallToActionViewModel.Reservations = new List<Reservation> { singleReservation };
            panelViewModel.Data.CallToActionViewModel.VacanciesViewModel = new VacanciesViewModel { Vacancies = new List<VacancyViewModel> { singleVacancy } };

            // Act
            sut.GetCallToActionViewName(panelViewModel);

            //Assert
            panelViewModel.ComponentName.Should().Be(ComponentConstants.VacancyClosed);
        }
    }
}
