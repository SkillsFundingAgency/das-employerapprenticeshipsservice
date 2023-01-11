namespace SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

public class SignEmployerAgreement
{
    public long AgreementId { get; set; }
    public long SignedById { get; set; }
    public string SignedByName { get; set; }
    public DateTime SignedDate { get; set; }
}