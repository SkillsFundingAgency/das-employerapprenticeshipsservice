using SFA.DAS.EmployerAccounts.Models.Recruit;

namespace SFA.DAS.EmployerAccounts.Queries.GetVacancies;

public class GetVacanciesResponse
{
    public IEnumerable<Vacancy> Vacancies { get; set; }
    public bool HasFailed { get; set; }
}