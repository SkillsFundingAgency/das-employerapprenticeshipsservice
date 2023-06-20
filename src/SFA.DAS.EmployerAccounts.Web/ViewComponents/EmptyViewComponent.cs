namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class EmptyViewComponent: ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        return View(model);
    }
}