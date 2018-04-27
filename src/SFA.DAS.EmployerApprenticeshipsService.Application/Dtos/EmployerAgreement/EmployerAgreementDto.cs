using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using System;

namespace SFA.DAS.EAS.Application.Dtos.EmployerAgreement
{
    public class EmployerAgreementDto
    {
        public virtual long Id { get; set; }
        public virtual string HashedAgreementId { get; set; }
        public virtual long LegalEntityId { get; set; }
        public virtual long AccountId { get; set; }
        public virtual string HashedAccountId { get; set; }
        public virtual int TemplateId { get; set; }
        public virtual EmployerAgreementStatus StatusId { get; set; }
        public virtual string SignedByName { get; set; }
        public virtual DateTime? SignedDate { get; set; }
        public virtual DateTime? ExpiredDate { get; set; }
        public virtual long? SignedById { get; set; }

        public AccountDto Account { get; set; }
        public LegalEntityDto LegalEntity { get; set; }
        public AgreementTemplateDto Template { get; set; }
    }
}
