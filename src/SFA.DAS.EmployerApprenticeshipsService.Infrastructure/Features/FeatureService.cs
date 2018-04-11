using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureService : AzureServiceBase<FeatureToggleConfiguration>, IFeatureService
    {
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.FeaturesV2";
        public static TimeSpan DefaultCacheTime { get; } = new TimeSpan(0, 0, 30, 0);
        public sealed override ILog Logger { get; set; }

        private readonly IInProcessCache _cacheProvider;
        private readonly IFeatureCacheFactory _featureCacheFactory;

        public FeatureService(IInProcessCache cacheProvider, IFeatureCacheFactory featureCacheFactory, ILog logger)
        {
            _cacheProvider = cacheProvider;
            _featureCacheFactory = featureCacheFactory;
            Logger = logger;
        }

        public async Task<Feature> GetFeature(FeatureType featureType)
        {
            var featureCache = await GetFeatureCache().ConfigureAwait(false);
            var feature = featureCache.GetFeature(featureType);

            return feature;
        }

        private async Task<IFeatureCache> GetFeatureCache()
        {
            var featureCache = _cacheProvider.Get<IFeatureCache>(nameof(FeatureCache));

            if (featureCache == null)
            {
                var features = await GetDataFromTableStorageAsync().ConfigureAwait(false);

                featureCache = _featureCacheFactory.Create(features.Data);

                _cacheProvider.Set(nameof(FeatureCache), featureCache, DefaultCacheTime);
            }

            return featureCache;
        }
    }
}