using System;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Features;

namespace SFA.DAS.EmployerAccounts.Features
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