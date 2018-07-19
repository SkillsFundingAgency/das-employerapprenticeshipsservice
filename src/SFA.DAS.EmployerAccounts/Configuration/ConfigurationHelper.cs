using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Configuration
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

        public static string CurrentEnvironmentName
        {
            get
            {
                var environmentName = System.Environment.GetEnvironmentVariable("DASENV");

                if (string.IsNullOrEmpty(environmentName))
                {
                    environmentName = ConfigurationManager.AppSettings["EnvironmentName"];
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

        public static bool IsEnvironmentAnyOf(params Environment[] environment)
        {
            return environment.Contains(CurrentEnvironment);
        }

        private static ConfigurationService CreateConfigurationService(string serviceName)
        {
            var storageConnectionString = ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"];
            var environmentName = CurrentEnvironmentName;
            var configurationRepository = new AzureTableStorageConfigurationRepository(storageConnectionString);
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, environmentName, "1.0"));

            return configurationService;
        }
    }
}