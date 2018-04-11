using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public interface IFeatureCacheFactory
    {
        IFeatureCache Create(IEnumerable<Feature> features);
    }
}