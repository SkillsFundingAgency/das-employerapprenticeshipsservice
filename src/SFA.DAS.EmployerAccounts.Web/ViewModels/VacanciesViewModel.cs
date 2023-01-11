namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class VacanciesViewModel
{
    public int VacancyCount => Vacancies?.Count() ?? 0;

    public IEnumerable<VacancyViewModel> Vacancies { get; set; }
}