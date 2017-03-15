using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Types.Events.LegalEntity
{
    public class LegalEntityCreatedEvent : IEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public string ResourceUri { get; set; }
    }
}
