using System;
using System.Linq;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.FeatureToggle;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class FeatureToggleFileBasedService : FileSystemRepository, IFeatureToggle
    {
        private readonly ICacheProvider _cacheProvider;

        public FeatureToggleFileBasedService(ICacheProvider cacheProvider) : base("Features")
        {
            _cacheProvider = cacheProvider;
        }

        public FeatureToggleLookup GetFeatures()
        {

            var features = _cacheProvider.Get<FeatureToggleLookup>(nameof(FeatureToggleLookup));
            if(features == null)
            {
                features = ReadFileByIdSync<FeatureToggleLookup>("features_data");
                if (features.Data != null && features.Data.Any())
                {
                    _cacheProvider.Set(nameof(FeatureToggleLookup),features,new TimeSpan(0,30,0));
                }
            }
            return features;
        }
    }
}
