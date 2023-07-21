namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class VacancyClosedViewComponent: ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        return View(model.CallToActionViewModel.VacanciesViewModel.Vacancies.First(m => m.Status == EmployerAccounts.Models.Recruit.VacancyStatus.Closed));
    }
}