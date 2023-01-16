namespace SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;

public class GetMinimumSignedAgreementVersionQuery : IRequest<GetMinimumSignedAgreementVersionResponse>
{
    public long AccountId { get; set; }
}