using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class FeatureService : AzureServiceBase<FeatureToggleConfiguration>, IFeatureService
    {
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.Features";
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

        public async Task<Feature> GetFeatureThatAllowsAccessToOperationAsync(string controllerName, string actionName)
        {
            var featureCache = await GetFeatureCache().ConfigureAwait(false);

            if (featureCache.TryGetControllerActionSubjectToFeature(controllerName, actionName, out var controllerAction))
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
                var toggledFeatures = await GetToggledFeaturesAsync().ConfigureAwait(false);

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