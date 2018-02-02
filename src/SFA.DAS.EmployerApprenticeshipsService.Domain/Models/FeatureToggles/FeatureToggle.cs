using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class FeatureToggle
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public List<string> Whitelist { get; set; }

        public FeatureToggle()
        {
            Whitelist = new List<string>();
        }
    }
}