namespace SFA.DAS.EmployerAccounts.Dtos;

public class EmployerAgreementDetailsDto
{
    public long Id { get; set; }
    public int TemplateId { get; set; }
    public string PartialViewName { get; set; }
    public string HashedAgreementId { get; set; }
    public int VersionNumber { get; set; }
}