using System;
using System.Linq;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggle;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class FeatureToggleService : AzureServiceBase<FeatureToggleLookup>, IFeatureToggle
    {
        private readonly ICacheProvider _cacheProvider;

        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.Features";
        public FeatureToggleService(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public virtual FeatureToggleLookup GetFeatures()
        {

            var features = _cacheProvider.Get<FeatureToggleLookup>(nameof(FeatureToggleLookup));
            if(features == null)
            {
                features = GetDataFromStorage();
                if (features.Data != null && features.Data.Any())
                {
                    _cacheProvider.Set(nameof(FeatureToggleLookup),features,new TimeSpan(0,30,0));
                }
            }
            return features;
        }
    }
}
