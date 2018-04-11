using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureCache : IFeatureCache
    {
        private readonly Dictionary<FeatureType, Feature> _features;

        public FeatureCache(IEnumerable<Feature> features)
        {
            _features = Enum.GetValues(typeof(FeatureType)).Cast<FeatureType>().ToDictionary(t => t, t => 
                features.SingleOrDefault(f => f.FeatureType == t) ??
                new Feature { FeatureType = t });
        }

        public Feature GetFeature(FeatureType featureType)
        {
            return _features[featureType];
        }
    }
}