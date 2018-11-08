using System.Configuration;
using System.Threading.Tasks;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using Environment = System.Environment;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public static class ConfigurationHelper
    {
        private static string CurrentEnvironmentName
        {
            get
            {
                var environmentName = Environment.GetEnvironmentVariable("DASENV");

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

        private static ConfigurationService CreateConfigurationService(string serviceName)
        {
            var storageConnectionString = ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"];
            var configurationRepository = new AzureTableStorageConfigurationRepository(storageConnectionString);
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(serviceName, CurrentEnvironmentName, "1.0"));

            return configurationService;
        }
    }
}