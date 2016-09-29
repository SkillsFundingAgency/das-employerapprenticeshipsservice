using System;
using System.Configuration;
using System.Linq;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.FeatureToggle;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class FeatureToggleFileBasedService : IFeatureToggle
    {
        private readonly ICacheProvider _cacheProvider;
        private const string ConfigurationName = "SFA.DAS.EmployerApprenticeshipsService.Features";

        public FeatureToggleFileBasedService(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public FeatureToggleLookup GetFeatures()
        {

            var features = _cacheProvider.Get<FeatureToggleLookup>(nameof(FeatureToggleLookup));
            if(features == null)
            {
                features = GetFeatureToggleListLookup();
                if (features.Data != null && features.Data.Any())
                {
                    _cacheProvider.Set(nameof(FeatureToggleLookup),features,new TimeSpan(0,30,0));
                }
            }
            return features;
        }

        private FeatureToggleLookup GetFeatureToggleListLookup()
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

            var config = configurationService.Get<FeatureToggleLookup>();

            return config;
        }

        private static IConfigurationRepository GetDataFromAzure()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository =
                    new AzureTableStorageConfigurationRepository(
                        CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }
    }

    
}
