using System;
using System.Configuration;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public abstract class AzureServiceBase<T>
    {
        public abstract string ConfigurationName { get; }
        protected IConfigurationRepository GetDataFromAzure()
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

        public virtual T GetDataFromStorage()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = GetDataFromAzure();
            var configurationService = new ConfigurationService(
                configurationRepository,
                new ConfigurationOptions(ConfigurationName, environment, "1.0"));

            var config = configurationService.Get<T>();

            return config;
        }
    }
}