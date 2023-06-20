namespace SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;

public class GetLastSignedAgreementRequest : IRequest<GetLastSignedAgreementResponse>
{
    public long AccountLegalEntityId { get; set; }
}