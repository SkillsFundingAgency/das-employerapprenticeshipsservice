namespace SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;

public class SignEmployerAgreementCommand : IRequest<SignEmployerAgreementCommandResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
    public DateTime SignedDate { get; set; }
    public string HashedAgreementId { get; set; }
}