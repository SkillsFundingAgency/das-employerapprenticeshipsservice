using System;

namespace SFA.DAS.EAS.Domain.Models.EmployerAgreement
{
    public class EmployerAgreementStatusView
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public long LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityAddress { get; set; }
        public DateTime? LegalEntityInceptionDate { get; set; }

        public string SignedByName { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? SignedExpiredDate { get; set; }

        public long? SignedAgreementId { get; set; }
        public int? SignedTemplateId { get; set; }
        public string SignedTemplatePartialViewName { get; set; }
        public string SignedHashedAgreementId { get; set; }
        public int? SignedVersion{ get; set; }


        public long? PendingAgreementId { get; set; }
        public int? PendingTemplateId { get; set; }
        public string PendingTemplatePartialViewName { get; set; }
        public string PendingHashedAgreementId { get; set; }
        public int? PendingVersion { get; set; }

        public string LegalEntityStatus { get; set; }
        public string Sector { get; set; }

        public bool HasPendingAgreement => PendingAgreementId.HasValue;
        public bool HasSignedAgreement => SignedAgreementId.HasValue;
    }
}