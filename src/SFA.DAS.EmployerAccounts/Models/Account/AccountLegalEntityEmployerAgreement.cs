using System;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Models.Account
{
    public class AccountLegalEntityEmployerAgreement
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
        public virtual long AccountId { get; set; }
        public virtual long LegalEntityId { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual long? SignedAgreementId { get; set; }
        public virtual int? SignedAgreementVersion { get; set; }
        public virtual long? PendingAgreementId { get; set; }
        public virtual int? PendingAgreementVersion { get; set; }
        public virtual string PublicHashedId { get; set; }
        public virtual DateTime? Deleted { get; set; }

        public virtual long EmployerAgreementId { get; set; }
        public virtual int TemplateId { get; set; }
        public virtual EmployerAgreementStatus StatusId { get; set; }
        public virtual string SignedByName { get; set; }
        public virtual DateTime? SignedDate { get; set; }
        public virtual long AccountLegalEntityId { get; set; }
        public virtual long? SignedById { get; set; }
        public virtual DateTime? ExpiredDate { get; set; }

    }
}
