using System;
using System.Linq;
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

        public static bool IsAnyOf(params DasEnvironment[] environment)
        {
            return environment.Contains(CurrentEnvironment);
        }

        public static DasEnvironment CurrentEnvironment
        {
            get
            {
                switch (CurrentEnvironmentName)
                {
                    case "LOCAL": return DasEnvironment.Local;
                    case "AT": return DasEnvironment.AT;
                    case "TEST": return DasEnvironment.Test;
                    case "PROD": return DasEnvironment.Prod;
                    case "DEMO": return DasEnvironment.Demo;
                    default: return DasEnvironment.Unknown;
                }
            }
        }

        public static string CurrentEnvironmentName
        {
            get
            {
                {
                    var environmentName = Environment.GetEnvironmentVariable("DASENV");

                    if (string.IsNullOrEmpty(environmentName))
                    {
                        environmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
                    }

                    return environmentName.ToUpperInvariant();
                }
            }
        }

        private static ConfigurationService CreateConfigurationService(string serviceName)
        {
            var environmentName = CurrentEnvironmentName;
            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, environmentName, "1.0"));

            return configurationService;
        }

        public static T GetConfigForService<T>(string serviceName)
        {
            var configurationService = CreateConfigurationService(serviceName);

            return configurationService.Get<T>();
        }
    }
}
