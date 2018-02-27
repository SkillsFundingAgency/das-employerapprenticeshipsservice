using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class FeatureToggleCollection
    {
        public FeatureToggleCollection()
        {
            Features = new List<FeatureToggle>();    
        }

        public List<FeatureToggle> Features { get; set; }
    }
}