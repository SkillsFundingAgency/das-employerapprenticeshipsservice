namespace SFA.DAS.EmployerAccounts.Queries.GetVacancies;

public class GetVacanciesRequest : IAsyncRequest<GetVacanciesResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}