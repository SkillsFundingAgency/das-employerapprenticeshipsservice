using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class FeatureCacheFactory : IFeatureCacheFactory
    {
        private readonly IControllerMetaDataService _controllerMetaDataService;

        public FeatureCacheFactory(IControllerMetaDataService controllerMetaDataService)
        {
            _controllerMetaDataService = controllerMetaDataService;
        }

        public IFeatureCache Create(Feature[] toggledFeatures)
        {
            return new FeatureCache(toggledFeatures, _controllerMetaDataService);
        }
    }
}