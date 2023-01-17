namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class CheckFundingViewComponent: ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        return View(model); 
    }
}