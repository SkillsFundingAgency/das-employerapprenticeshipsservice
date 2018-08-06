
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Features;

namespace SFA.DAS.EmployerAccounts.Features
{
    public interface IFeatures
    {
        Feature GetFeature(FeatureType featureType);
    }
}