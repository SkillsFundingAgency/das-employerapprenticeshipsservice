namespace SFA.DAS.EmployerAccounts.Queries.GetAccountStats;

public class GetAccountStatsQuery : IRequest<GetAccountStatsResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}