namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

public class GetAccountEmployerAgreementsRequest : IAsyncRequest<GetAccountEmployerAgreementsResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}