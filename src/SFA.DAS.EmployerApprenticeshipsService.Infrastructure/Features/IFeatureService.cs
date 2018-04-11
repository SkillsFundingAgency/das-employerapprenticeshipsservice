using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public interface IFeatureService
    {
        Feature GetFeature(FeatureType featureType);
    }
}