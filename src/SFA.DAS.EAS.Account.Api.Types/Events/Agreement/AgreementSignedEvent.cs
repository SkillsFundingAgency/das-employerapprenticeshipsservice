using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Types.Events.Agreement
{
    public class AgreementSignedEvent: IEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public string ResourceUrl { get; set; }
    }
}
