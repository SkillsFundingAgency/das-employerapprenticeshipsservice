using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap;
using StructureMap.Pipeline;
using IConfiguration = SFA.DAS.EAS.Domain.Interfaces.IConfiguration;


namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
{
    public class MessageSubscriberPolicy<T> : ConfiguredInstancePolicy where T : IConfiguration
    {
        private readonly string _serviceName;

        public MessageSubscriberPolicy(string serviceName)
        {
            _serviceName = serviceName;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var subscriberFactory = GetMessageSubscriberFactoryParameter(instance);

            if (subscriberFactory == null) return;

            var environment = GetEnvironmentName();
            
            var messageQueueConnectionString = GetMessageQueueConnectionString(environment);

            if (string.IsNullOrEmpty(messageQueueConnectionString))
            {
                var groupFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/EAS_Queues/";
                var factory = new FileSystemMessageSubscriberFactory(groupFolder);

                instance.Dependencies.AddForConstructorParameter(subscriberFactory, factory);
            }
            else
            {
                var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName(instance.Constructor.DeclaringType);

                var factory = new TopicSubscriberFactory(messageQueueConnectionString, subscriptionName, new NLogLogger(typeof(TopicSubscriberFactory)));

                instance.Dependencies.AddForConstructorParameter(subscriberFactory, factory);
            }   
        }

        private static ParameterInfo GetMessageSubscriberFactoryParameter(IConfiguredInstance instance)
        {
            var factory = instance?.Constructor?
                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessageSubscriberFactory));

            return factory;
        }

        private static string GetEnvironmentName()
        {
            var environment = System.Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }
            return environment;
        }

        private string GetMessageQueueConnectionString(string environment)
        {
            var configurationService = new ConfigurationService(GetConfigurationRepository(),
                new ConfigurationOptions(_serviceName, environment, "1.0"));

            var config = configurationService.Get<T>();

            var messageQueueConnectionString = config.MessageServiceBusConnectionString;
            return messageQueueConnectionString;
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }
    }
}