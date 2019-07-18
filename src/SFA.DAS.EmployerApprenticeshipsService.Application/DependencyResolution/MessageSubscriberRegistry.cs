using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Messaging;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class MessageSubscriberRegistry : Registry
    {
        public MessageSubscriberRegistry()
        {
            Policies.Add(new MessageSubscriberPolicy<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName));

            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(Constants.ServiceNamespace));
                s.AddAllTypesOf<IMessageProcessor>();
            });
        }
    }
}