using System.Threading.Tasks;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public static class ConfigurationHelper
    {
        private static string CurrentEnvironmentName
        {
            get
            {
                var environmentName = System.Environment.GetEnvironmentVariable("DASENV");

                if (string.IsNullOrEmpty(environmentName))
                {
                    environmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
                }

                return environmentName.ToUpperInvariant();
            }
        }

        public static T GetConfiguration<T>(string serviceName)
        {
            var configurationService = CreateConfigurationService(serviceName);
            return configurationService.Get<T>();
        }

        public static Task<T> GetConfigurationAsync<T>(string serviceName)
        {
            var configurationService = CreateConfigurationService(serviceName);
            return configurationService.GetAsync<T>();
        }

        private static ConfigurationService CreateConfigurationService(string serviceName)
        {
            var storageConnectionString = CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString");
            var configurationRepository = new AzureTableStorageConfigurationRepository(storageConnectionString);
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, CurrentEnvironmentName, "1.0"));

            return configurationService;
        }
    }
}