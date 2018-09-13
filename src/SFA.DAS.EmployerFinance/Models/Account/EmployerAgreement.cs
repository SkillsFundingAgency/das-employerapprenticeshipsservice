using System;
using SFA.DAS.EmployerFinance.Models.EmployerAgreement;

namespace SFA.DAS.EmployerFinance.Models.Account
{
    public class EmployerAgreement
    {
        public virtual long Id { get; set; }
        public virtual Account Account { get; set; }
        public virtual long AccountId { get; set; }
        public virtual DateTime? ExpiredDate { get; set; }
        public virtual LegalEntity LegalEntity { get; set; }
        public virtual long LegalEntityId { get; set; }
        public virtual long? SignedById { get; set; }
        public virtual string SignedByName { get; set; }
        public virtual DateTime? SignedDate { get; set; }
        public virtual EmployerAgreementStatus StatusId { get; set; }
        public virtual AgreementTemplate Template { get; set; }
        public virtual int TemplateId { get; set; }
    }
}