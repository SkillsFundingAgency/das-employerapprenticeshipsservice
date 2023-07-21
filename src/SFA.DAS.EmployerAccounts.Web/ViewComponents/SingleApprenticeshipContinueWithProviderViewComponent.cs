namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class SingleApprenticeshipContinueWithProviderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        model.CallToActionViewModel.Cohorts.Single().Apprenticeships = new List<ApprenticeshipViewModel>()
        {
            new ApprenticeshipViewModel()
            {
                CourseName = model.CallToActionViewModel.Cohorts?.Single()?.CohortApprenticeshipsCount > 0 ? model.CallToActionViewModel.Cohorts?.Single()?.Apprenticeships?.Single()?.CourseName : string.Empty,
                HashedCohortId = model.CallToActionViewModel.Cohorts?.Single().HashedCohortId,
                TrainingProvider = model.CallToActionViewModel.Cohorts?.Single().TrainingProvider.First()
            }
        };
        return View(model.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single());
    }
}