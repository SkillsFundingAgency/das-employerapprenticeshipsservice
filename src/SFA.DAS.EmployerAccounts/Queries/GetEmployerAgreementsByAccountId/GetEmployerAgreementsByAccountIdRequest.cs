namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;

public class GetEmployerAgreementsByAccountIdRequest : IRequest<GetEmployerAgreementsByAccountIdResponse>
{
    public long AccountId { get; set; }
}