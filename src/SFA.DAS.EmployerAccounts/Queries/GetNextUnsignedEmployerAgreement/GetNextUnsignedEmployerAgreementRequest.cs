namespace SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;

public class GetNextUnsignedEmployerAgreementRequest : IRequest<GetNextUnsignedEmployerAgreementResponse>
{
    public long AccountId { get; set; }
    public string ExternalUserId { get; set; }
}