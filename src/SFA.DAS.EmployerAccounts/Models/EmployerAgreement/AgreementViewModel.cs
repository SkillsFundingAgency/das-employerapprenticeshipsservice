namespace SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

public class AgreementViewModel
{
    public long Id { get; set; }
    public DateTime? SignedDate { get; set; }
    public string SignedByName { get; set; }
    public EmployerAgreementStatus Status { get; set; }
    public int TemplateVersionNumber { get; set; }
}