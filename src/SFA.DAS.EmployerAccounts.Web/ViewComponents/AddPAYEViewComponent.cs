namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class AddPAYEViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        return View(model);
    }
}