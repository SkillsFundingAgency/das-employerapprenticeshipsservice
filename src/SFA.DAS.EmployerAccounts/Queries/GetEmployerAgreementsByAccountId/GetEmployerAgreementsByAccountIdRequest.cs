namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;

public class GetEmployerAgreementsByAccountIdRequest : IAsyncRequest<GetEmployerAgreementsByAccountIdResponse>
{
    public long AccountId { get; set; }
}