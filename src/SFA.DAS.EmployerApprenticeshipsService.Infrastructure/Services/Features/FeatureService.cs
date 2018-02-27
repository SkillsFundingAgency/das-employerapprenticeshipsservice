using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class FeatureService : AzureServiceBase<FeatureToggleConfiguration>, IFeatureService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IFeatureCacheFactory _featureCacheFactory;

        public sealed override ILog Logger { get; set; }

        public static TimeSpan DefaultCacheTime { get; } = new TimeSpan(0, 0, 30, 0);

        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.Features";

        public FeatureService(
            ICacheProvider cacheProvider, 
            ILog logger,
            IFeatureCacheFactory featureCacheFactory)
        {
            _cacheProvider = cacheProvider;
            _featureCacheFactory = featureCacheFactory;
            Logger = logger;
        }

        public async Task<string[]> GetWhitelistForOperationAsync(OperationContext context)
        {
            var configuration = await GetFeatureCache();

            if (configuration.TryGetControllerActionSubjectToFeature(context, out var controllerAction))
            {
                return controllerAction.WhiteList;
            }

            return null;
        }

        public async Task<Feature> GetFeatureThatAllowsAccessToOperationAsync(OperationContext context)
        {
            var featureCache = await GetFeatureCache();

            if (featureCache.TryGetControllerActionSubjectToFeature(context, out var controllerAction))
            {
                return controllerAction.Feature;
            }

            return null;
        }

        private async Task<IFeatureCache> GetFeatureCache()
        {

            var featureCache = _cacheProvider.Get<IFeatureCache>(nameof(FeatureCache));
            if (featureCache == null)
            {
                var toggledFeatures = await GetToggledFeaturesAsync();

                featureCache = _featureCacheFactory.Create(toggledFeatures.Data);
                _cacheProvider.Set(nameof(FeatureCache), featureCache, DefaultCacheTime );
            }

            return featureCache;
        }

        private Task<FeatureToggleConfiguration> GetToggledFeaturesAsync()
        {
            return GetDataFromTableStorageAsync();
        }
    }
}
