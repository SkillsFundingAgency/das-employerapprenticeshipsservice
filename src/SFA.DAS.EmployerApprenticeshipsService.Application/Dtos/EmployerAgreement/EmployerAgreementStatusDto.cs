using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Application.Dtos.EmployerAgreement
{
    public class EmployerAgreementStatusDto
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public LegalEntityDto LegalEntity { get; set; }
        public SignedEmployerAgreementDetailsDto Signed { get; set; }
        public PendingEmployerAgreementDetailsDto Pending { get; set; }
        public bool HasPendingAgreement => Pending != null;
        public bool HasSignedAgreement => Signed!= null;
    }
}