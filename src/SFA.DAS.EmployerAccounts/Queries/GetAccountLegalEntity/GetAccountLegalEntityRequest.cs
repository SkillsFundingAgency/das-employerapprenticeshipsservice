namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntity;

public class GetAccountLegalEntityRequest : IRequest<GetAccountLegalEntityResponse>
{
    public long AccountLegalEntityId { get; set; }
}