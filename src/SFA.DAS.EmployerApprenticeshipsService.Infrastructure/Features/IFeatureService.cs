using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public interface IFeatureService
    {
        Task<Feature> GetFeature(FeatureType featureType);
    }
}