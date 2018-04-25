using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureService : IFeatureService
    {
        private readonly Dictionary<FeatureType, Feature> _features;

        public FeatureService(FeaturesConfiguration configuration)
        {
            _features = Enum.GetValues(typeof(FeatureType)).Cast<FeatureType>().ToDictionary(t => t, t =>
                configuration.Data.SingleOrDefault(f => f.FeatureType == t) ??
                new Feature { FeatureType = t });
        }

        public Feature GetFeature(FeatureType featureType)
        {
            return _features[featureType];
        }
    }
}