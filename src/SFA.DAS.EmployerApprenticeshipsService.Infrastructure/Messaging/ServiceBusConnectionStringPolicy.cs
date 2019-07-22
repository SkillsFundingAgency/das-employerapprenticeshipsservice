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
        private readonly T _configuration;

        protected ServiceBusConnectionStringPolicy(T configuration)
        {
            _configuration = configuration;
        }

        protected string GetConnectionString(IConfiguredInstance instance)
        {
            var attribute = instance.PluggedType.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(ServiceBusConnectionStringAttribute));

            if (attribute == null)
            {
                return _configuration.MessageServiceBusConnectionString;
            }

            var connectionStringName = attribute.ConstructorArguments.Select(a => a.Value).Cast<string>().Single();

            if (!_configuration.ServiceBusConnectionStrings.TryGetValue(connectionStringName, out var connectionString))
            {
                throw new InvalidOperationException($"Cannot find service bus connection string named '{connectionStringName}' in configuration.");
            }

            return connectionString;
        }
    }
}