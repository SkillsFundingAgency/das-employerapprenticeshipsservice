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
            var configurationService = CreateConfigurationService(serviceName);
            var config = configurationService.Get<T>();

            return config;
        }

        public static async Task<T> GetConfigurationAsync<T>(string serviceName)
        {
            var configurationService = CreateConfigurationService(serviceName);
            return await configurationService.GetAsync<T>().ConfigureAwait(false);
        }

        private static ConfigurationService CreateConfigurationService(string serviceName)
        {
            var environmentName = Environment.GetEnvironmentVariable("DASENV");

            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, environmentName, "1.0"));

            return configurationService;
        }
    }
}
