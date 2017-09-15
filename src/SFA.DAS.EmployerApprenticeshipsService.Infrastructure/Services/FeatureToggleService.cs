using System;
using System.Linq;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggle;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.EnvironmentInfo;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class FeatureToggleService : AzureServiceBase, IFeatureToggle
    {
        private readonly IConfigurationInfo<FeatureToggleLookup> _configInfo;

        private readonly ICacheProvider _cacheProvider;
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.Features";
        public FeatureToggleService(ICacheProvider cacheProvider, ILog logger, IConfigurationInfo<FeatureToggleLookup> configInfo)
        {
            _cacheProvider = cacheProvider;
            Logger = logger;
            _configInfo= configInfo;
        }

        public virtual FeatureToggleLookup GetFeatures()
        {

            var features = _cacheProvider.Get<FeatureToggleLookup>(nameof(FeatureToggleLookup));
            if(features == null)
            {
                features = _configInfo.GetConfiguration(ConfigurationName);
                if (features.Data != null && features.Data.Any())
                {
                    _cacheProvider.Set(nameof(FeatureToggleLookup),features,new TimeSpan(0,30,0));
                }
            }
            return features;
        }
    }
}
