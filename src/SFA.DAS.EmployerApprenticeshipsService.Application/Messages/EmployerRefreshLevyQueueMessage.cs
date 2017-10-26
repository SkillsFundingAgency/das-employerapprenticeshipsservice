using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EAS.Application.Messages
{
    [MessageGroup("get_employer_levy2")]
    public class EmployerRefreshLevyQueueMessage 
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
    }
}
