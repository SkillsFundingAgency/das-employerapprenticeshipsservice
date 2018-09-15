using System;
using System.Linq;
using SFA.DAS.Configuration;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Infrastructure.Messaging
{
    public abstract class ServiceBusConnectionStringPolicy<T> : ConfiguredInstancePolicy where T : ITopicMessageSubscriberConfiguration
    {
        protected readonly string ServiceName;

        protected ServiceBusConnectionStringPolicy(string serviceName)
        {
            ServiceName = serviceName;
        }

        protected string GetConnectionString(IConfiguredInstance instance)
        {
            var attribute = instance.PluggedType.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(ServiceBusConnectionStringAttribute));
            var config = ConfigurationHelper.GetConfiguration<T>(ServiceName);

            if (attribute == null)
            {
                return config.MessageServiceBusConnectionString;
            }

            var connectionStringName = attribute.ConstructorArguments.Select(a => a.Value).Cast<string>().Single();

            if (!config.ServiceBusConnectionStrings.TryGetValue(connectionStringName, out var connectionString))
            {
                throw new InvalidOperationException($"Cannot find service bus connection string named '{connectionStringName}' in configuration.");
            }

            return connectionString;
        }
    }
}