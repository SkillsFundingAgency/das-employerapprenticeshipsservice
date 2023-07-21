namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class SingleApprenticeshipWithTrainingProviderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        return View(model.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single());
    }
}