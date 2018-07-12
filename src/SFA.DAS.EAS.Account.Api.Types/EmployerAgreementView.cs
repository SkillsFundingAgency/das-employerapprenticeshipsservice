using System;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class EmployerAgreementView : IAccountResource
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public EmployerAgreementStatus Status { get; set; }
        public string SignedByName { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityAddress { get; set; }
        public DateTime? LegalEntityInceptionDate { get; set; }
        public int TemplateId { get; set; }
        public string TemplatePartialViewName { get; set; }
        public string HashedAgreementId { get; set; }
        public string LegalEntityStatus { get; set; }
        public string Sector { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
    }
}
