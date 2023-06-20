namespace SFA.DAS.EmployerAccounts.Dtos;

public class VacanciesSummary
{
    public List<VacancySummary> Vacancies { get; set; }
    public int PageSize { get; set; }
    public int PageNo { get; set; }
    public int TotalResults { get; set; }
    public int TotalPages { get; set; }

    public VacanciesSummary()
    {
        Vacancies = new List<VacancySummary>();
    }
}