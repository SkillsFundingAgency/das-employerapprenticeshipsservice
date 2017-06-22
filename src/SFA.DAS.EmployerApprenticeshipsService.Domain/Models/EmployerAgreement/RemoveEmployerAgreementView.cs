using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.EmployerAgreement
{
    public class RemoveEmployerAgreementView
    {
        public string Name { get; set; }
        public EmployerAgreementStatus Status { get; set; }
        public long Id { get; set; }
        public string HashedAgreementId { get; set; }
        public string HashedAccountId { get; set; }
        public bool CanBeRemoved { get; set; }
        public string LegalEntityCode { get; set; }
    }
}