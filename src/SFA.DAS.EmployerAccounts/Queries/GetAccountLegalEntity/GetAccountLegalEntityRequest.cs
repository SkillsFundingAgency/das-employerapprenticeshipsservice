namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntity;

public class GetAccountLegalEntityRequest : IAsyncRequest<GetAccountLegalEntityResponse>
{
    public long AccountLegalEntityId { get; set; }
}