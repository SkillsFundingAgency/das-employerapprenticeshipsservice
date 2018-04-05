using System.Collections.Generic;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public interface ITopicMessageSubscriberConfiguration : ITopicConfiguration
    {
        Dictionary<string, string> ServiceBusConnectionStrings { get; set; }
    }
}