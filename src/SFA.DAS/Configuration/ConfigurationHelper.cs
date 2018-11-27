using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Configuration
{
    public static class ConfigurationHelper
    {
        public static Environment CurrentEnvironment
        {
            get
            {
                switch (CurrentEnvironmentName)
                {
                    case "LOCAL": return Environment.Local;
                    case "AT": return Environment.At;
                    case "TEST": return Environment.Test;
                    case "PROD": return Environment.Prod;
                    case "MO": return Environment.Mo;
                    case "DEMO": return Environment.Demo;
                    default: return Environment.Unknown;
                }
            }
        }

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
            var result = configurationService.Get<T>();
            return result;
        }

        public static Task<T> GetConfigurationAsync<T>(string serviceName)
        {
            var configurationService = CreateConfigurationService(serviceName);
            return configurationService.GetAsync<T>();
        }

        public static bool IsEnvironmentAnyOf(params Environment[] environment)
        {
            return environment.Contains(CurrentEnvironment);
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