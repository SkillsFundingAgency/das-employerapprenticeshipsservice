namespace SFA.DAS.EmployerAccounts.Queries.GetAccountStats;

public class GetAccountStatsQuery : IAsyncRequest<GetAccountStatsResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}