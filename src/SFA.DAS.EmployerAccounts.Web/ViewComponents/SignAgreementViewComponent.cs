namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class SignAgreementViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        return View(model);
    }
}