using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
{
    public static class ConfigurationHelper
    {
        public static T GetConfiguration<T>(string serviceName)
        {
            var environmentName = GetEnvironmentName();
            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, environmentName, "1.0"));
            var configuration = configurationService.Get<T>();

            return configuration;
        }

        public static string GetEnvironmentName()
        {
        }

        private static ConfigurationService CreateConfigurationService(string serviceName)
        {
            var environmentName = Environment.GetEnvironmentVariable("DASENV");

            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            return environmentName;
            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, environmentName, "1.0"));

            return configurationService;
        }   
    }
}
