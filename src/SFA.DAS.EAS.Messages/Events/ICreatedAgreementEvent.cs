using NServiceBus;
using System;

namespace SFA.DAS.EAS.Messages.Events
{
    public interface ICreatedAgreementEvent : IEvent
    {
        long AccountId { get; set; }
        DateTime Created { get; set; }
        string UserName { get; set; }
        Guid UserRef { get; set; }
        string OrganisationName { get; set; }
        long AgreementId { get; set; }
        long LegalEntityId { get; set; }
    }
}
