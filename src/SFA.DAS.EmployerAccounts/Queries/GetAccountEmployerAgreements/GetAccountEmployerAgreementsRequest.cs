namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

public class GetAccountEmployerAgreementsRequest : IRequest<GetAccountEmployerAgreementsResponse>
{
    public long AccountId { get; set; }
    public string ExternalUserId { get; set; }
}