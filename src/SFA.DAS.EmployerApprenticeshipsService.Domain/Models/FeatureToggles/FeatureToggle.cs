using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class FeatureToggle
    {
        public Feature Feature { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public WhiteList Whitelist { get; set; }

        public FeatureToggle()
        {
            Whitelist = new WhiteList();
        }
    }
}