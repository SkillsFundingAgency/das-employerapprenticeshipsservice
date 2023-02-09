namespace SFA.DAS.EmployerAccounts.Queries.GetAccountStats;

public class GetAccountStatsQuery : IRequest<GetAccountStatsResponse>
{
    public long AccountId { get; set; }
    public string ExternalUserId { get; set; }
}