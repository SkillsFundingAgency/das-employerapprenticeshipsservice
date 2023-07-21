using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Dtos;

public class EmployerAgreementDto
{
    public long Id { get; set; }
    public DateTime? ExpiredDate { get; set; }
    public long? SignedById { get; set; }
    public string SignedByName { get; set; }
    public DateTime? SignedDate { get; set; }
    public long AccountLegalEntityId { get; set; }
    public AccountLegalEntityDto AccountLegalEntity { get; set; }
    public EmployerAgreementStatus StatusId { get; set; }
    public AgreementTemplateDto Template { get; set; }        
    public string HashedAgreementId { get; set; }
    public string HashedAccountId { get; set; }
    public string HashedLegalEntityId { get; set; }
    public bool OrganisationLookupPossible { get; set; }
    public  int TemplateId { get; set; }
}