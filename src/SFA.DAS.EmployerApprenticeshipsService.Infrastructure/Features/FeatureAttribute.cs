using System;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class FeatureAttribute : Attribute
    {
        public FeatureType FeatureType { get; set; }

        public FeatureAttribute(FeatureType featureType)
        {
            FeatureType = featureType;
        }
    }
}