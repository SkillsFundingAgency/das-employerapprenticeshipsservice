namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;

public class GetEmployerAgreementRequest : IRequest<GetEmployerAgreementResponse>
{
    public string HashedAccountId { get; set; }

    public string HashedAgreementId { get; set; }

    public string ExternalUserId { get; set; }
}