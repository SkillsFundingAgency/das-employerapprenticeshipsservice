using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class VacanciesViewModel
    {
        public int VacancyCount { get; set; }

        public IEnumerable<VacancyViewModel> Vacancies { get; set; }
    }
}