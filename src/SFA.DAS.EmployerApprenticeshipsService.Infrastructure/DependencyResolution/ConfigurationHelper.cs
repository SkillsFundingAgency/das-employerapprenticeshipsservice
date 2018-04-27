using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
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
                    case "AT": return Environment.AT;
                    case "TEST": return Environment.Test;
                    case "PROD": return Environment.Prod;
                    case "DEMO": return Environment.Demo;
                    default: return Environment.Unknown;
                }
            }
        }

        public static string CurrentEnvironmentName
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
            var configuration = configurationService.Get<T>();

            return configuration;
        }

        public static Task<T> GetConfigurationAsync<T>(string serviceName)
        {
            return Task.Run(async () =>
            {
                var configurationService = CreateConfigurationService(serviceName);
                // HACK: The das config service continues on the sync context which deadlocks as ASP is waiting on the config load task to complete on the asp sync context
                // There is a PR to fix this - when the updated nuget package is available the outer task can be removed leaving only the following line of code
                return await configurationService.GetAsync<T>().ConfigureAwait(false);
            });
        }

        public static bool IsAnyOf(params Environment[] environment)
        {
            return environment.Contains(CurrentEnvironment);
        }

        private static ConfigurationService CreateConfigurationService(string serviceName)
        {
            var environmentName = CurrentEnvironmentName;
            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, environmentName, "1.0"));

            return configurationService;
        }
    }
}