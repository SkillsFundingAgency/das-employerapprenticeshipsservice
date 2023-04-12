namespace SFA.DAS.EmployerAccounts.Dtos;

public class EmployerAgreementStatusDto
{
    public long AccountId { get; set; }
    public string HashedAccountId { get; set; }
    public AccountSpecificLegalEntityDto LegalEntity { get; set; }
    public SignedEmployerAgreementDetailsDto Signed { get; set; }
    public EmployerAgreementDetailsDto Pending { get; set; }
    public bool HasPendingAgreement => Pending != null;
    public bool HasSignedAgreement => Signed!= null;
}