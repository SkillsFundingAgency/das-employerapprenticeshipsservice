using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureCacheFactory : IFeatureCacheFactory
    {
        public IFeatureCache Create(IEnumerable<Feature> features)
        {
            return new FeatureCache(features);
        }
    }
}