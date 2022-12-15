using System;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class CreatedAgreementEvent
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
        public string OrganisationName { get; set; }
        public long AgreementId { get; set; }
        public long LegalEntityId { get; set; }
        public DateTime Created { get; set; }
    }
}
