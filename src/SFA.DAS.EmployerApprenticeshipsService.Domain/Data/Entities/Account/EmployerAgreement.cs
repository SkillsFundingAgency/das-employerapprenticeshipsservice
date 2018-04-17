using System;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Domain.Data.Entities.Account
{
    public class EmployerAgreement
    {
        public virtual long Id { get; set; }
        public virtual long LegalEntityId { get; set; }
        public virtual long AccountId { get; set; }
        public virtual int TemplateId { get; set; }
        public virtual EmployerAgreementStatus StatusId { get; set; }
        public virtual string SignedByName { get; set; }
        public virtual DateTime? SignedDate { get; set; }
        public virtual DateTime? ExpiredDate { get; set; }
        public virtual long? SignedById { get; set; }

        public Account Account { get; set; }
        public LegalEntity LegalEntity { get; set; }
        public AgreementTemplate Template { get; set; }
    }
}