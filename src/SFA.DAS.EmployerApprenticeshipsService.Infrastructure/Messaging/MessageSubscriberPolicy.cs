using System;
using System.Linq;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Infrastructure.Messaging
{
    public class MessageSubscriberPolicy<T> : ServiceBusConnectionStringPolicy<T> where T : ITopicMessageSubscriberConfiguration
    {
        public MessageSubscriberPolicy(string serviceName)
            : base(serviceName)
        {
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var factoryParameter = instance?.Constructor?.GetParameters().FirstOrDefault(p => p.ParameterType == typeof(IMessageSubscriberFactory));

            if (factoryParameter == null)
            {
                return;
            }
            
            var connectionString = GetConnectionString(instance);
            var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName(instance.Constructor.DeclaringType);
            var factory = new TopicSubscriberFactory(connectionString, subscriptionName, new NLogLogger(typeof(TopicSubscriberFactory)));

            instance.Dependencies.AddForConstructorParameter(factoryParameter, factory);
        }
    }
}