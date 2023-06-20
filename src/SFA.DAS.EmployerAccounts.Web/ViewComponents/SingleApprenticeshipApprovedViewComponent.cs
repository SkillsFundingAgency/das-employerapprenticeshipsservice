namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class SingleApprenticeshipApprovedViewComponent: ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        return View(model.CallToActionViewModel.Apprenticeships.First());
    }
}