using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public interface IFeatureCache
    {
        Feature GetFeature(FeatureType featureType);
    }
}