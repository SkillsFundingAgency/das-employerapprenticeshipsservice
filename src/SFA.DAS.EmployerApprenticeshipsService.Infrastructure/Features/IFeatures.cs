using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public interface IFeatures
    {
        Feature GetFeature(FeatureType featureType);
    }
}