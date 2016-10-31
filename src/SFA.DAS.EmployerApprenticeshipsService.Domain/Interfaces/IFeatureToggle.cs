using SFA.DAS.EAS.Domain.Models.FeatureToggle;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IFeatureToggle
    {
        FeatureToggleLookup GetFeatures();
    }
}
