namespace SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;

public class GetNextUnsignedEmployerAgreementRequest : IRequest<GetNextUnsignedEmployerAgreementResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}