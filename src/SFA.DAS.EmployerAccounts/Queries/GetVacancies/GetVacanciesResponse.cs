using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.Recruit;

namespace SFA.DAS.EmployerAccounts.Queries.GetVacancies
{
    public class GetVacanciesResponse
    {
        public IEnumerable<Vacancy> Vacancies { get; set; }
    }
}
