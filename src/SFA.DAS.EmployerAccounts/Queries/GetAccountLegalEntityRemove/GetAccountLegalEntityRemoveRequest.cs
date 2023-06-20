namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;

public class GetAccountLegalEntityRemoveRequest : IRequest<GetAccountLegalEntityRemoveResponse>
{
    public string HashedAccountId { get; set; }
    public string UserId { get; set; }
    public string HashedAccountLegalEntityId { get; set; }
}