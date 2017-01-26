using System;

namespace SFA.DAS.EAS.Domain
{
    public class CreateAccountResult
    {
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public long EmployerAgreementId { get; set; }
    }
}
