using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Types.Events
{
    public class PayeSchemeAddedEvent : IEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public string ResourceUri { get; set; }
    }
}
