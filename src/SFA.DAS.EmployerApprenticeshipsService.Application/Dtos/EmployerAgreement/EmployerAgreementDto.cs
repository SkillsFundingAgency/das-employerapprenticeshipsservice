using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using System;

namespace SFA.DAS.EAS.Application.Dtos.EmployerAgreement
{
    public class EmployerAgreementDto
    {
        public long Id { get; set; }
        public string HashedAgreementId { get; set; }
        public long LegalEntityId { get; set; }
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public int TemplateId { get; set; }
        public EmployerAgreementStatus StatusId { get; set; }
        public string SignedByName { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long? SignedById { get; set; }

        public AccountDto Account { get; set; }
        public LegalEntityDto LegalEntity { get; set; }
        public AgreementTemplateDto Template { get; set; }
    }
}
