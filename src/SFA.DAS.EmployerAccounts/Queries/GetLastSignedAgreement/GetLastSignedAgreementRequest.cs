namespace SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;

public class GetLastSignedAgreementRequest : IAsyncRequest<GetLastSignedAgreementResponse>
{
    public long AccountLegalEntityId { get; set; }
}