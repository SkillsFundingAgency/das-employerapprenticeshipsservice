namespace SFA.DAS.EmployerAccounts.Models.Account;

public class CreateAccountResult
{
    public long AccountId { get; set; }
    public long LegalEntityId { get; set; }
    public long EmployerAgreementId { get; set; }
    public int AgreementVersion { get; set; }
    public long AccountLegalEntityId { get; set; }
}