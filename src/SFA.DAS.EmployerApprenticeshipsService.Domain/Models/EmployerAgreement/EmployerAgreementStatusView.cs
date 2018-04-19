using System;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Domain.Models.EmployerAgreement
{
    public class EmployerAgreementDetails
    {
        public long? Id { get; set; }
        public int? TemplateId { get; set; }
        public string PartialViewName { get; set; }
        public string HashedAgreementId { get; set; }
        public int? VersionNumber { get; set; }
    }

    public class SignedEmployerAgreementDetails : EmployerAgreementDetails
    {
        public string SignedByName { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }

    public class PendingEmployerAgreementDetails : EmployerAgreementDetails
    {
    }

    public class EmployerAgreementStatusView
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public LegalEntity LegalEntity { get; set; }
        public SignedEmployerAgreementDetails Signed { get; set; }
        public PendingEmployerAgreementDetails Pending { get; set; }
        public bool HasPendingAgreement => Pending != null && Pending.Id.HasValue;
        public bool HasSignedAgreement => Signed!= null && Signed.Id.HasValue;
    }
}