namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class SingleApprenticeshipContinueSetupViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        return View(model.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single());
    }
}