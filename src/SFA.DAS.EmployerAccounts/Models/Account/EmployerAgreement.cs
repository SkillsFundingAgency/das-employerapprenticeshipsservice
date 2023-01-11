using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Models.Account;

public class EmployerAgreement
{
    public virtual long Id { get; set; }
    public virtual DateTime? ExpiredDate { get; set; }
    public virtual long? SignedById { get; set; }
    public virtual string SignedByName { get; set; }
    public virtual DateTime? SignedDate { get; set; }
    public virtual long AccountLegalEntityId { get; set; }
    public virtual AccountLegalEntity AccountLegalEntity { get; set; }
    public virtual EmployerAgreementStatus StatusId { get; set; }
    public virtual AgreementTemplate Template { get; set; }
    public virtual int TemplateId { get; set; }
    public virtual string SignedByEmail { get ; set ; }
}