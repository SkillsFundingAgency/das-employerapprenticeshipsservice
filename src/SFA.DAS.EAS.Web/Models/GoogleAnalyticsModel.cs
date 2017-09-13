using System;
using System.Configuration;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Models
{
    public class GoogleAnalyticsModel
    {
        private const string ServiceName = "SFA.DAS.GoogleAnalytics";
        private const string ServiceNamespace = "SFA.DAS";

        private static GoogleAnalyticsModel _instance;

        private GoogleAnalyticsModel()
        {
            GetConfiguration();
        }

        public static GoogleAnalyticsModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GoogleAnalyticsModel();
                }
                return _instance;
            }
        }

        public string GoogleHeaderUrl { get; private set; }
        public string GoogleBodyUrl { get; private set; }

        public void GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));

            PopulateGoogleEnvironmentDetails(configurationService.Get<GoogleAnalyticsSnippets>());
        }

        private void PopulateGoogleEnvironmentDetails(GoogleAnalyticsSnippets environmentConfig)
        {
            GoogleHeaderUrl = environmentConfig.GoogleAnalyticsValues.GoogleHeaderUrl;
            GoogleBodyUrl = environmentConfig.GoogleAnalyticsValues.GoogleBodyUrl;
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