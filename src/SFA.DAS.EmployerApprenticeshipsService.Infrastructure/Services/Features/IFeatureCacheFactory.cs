using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public interface IFeatureCacheFactory
    {
        IFeatureCache Create(Feature[] toggledFeatures);
    }
}