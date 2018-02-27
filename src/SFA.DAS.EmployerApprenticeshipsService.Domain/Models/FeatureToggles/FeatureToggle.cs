using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class FeatureToggle
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<ControllerAction> Actions { get; set; }
        public WhiteList Whitelist { get; set; }

        public FeatureToggle()
        {
            Actions = new List<ControllerAction>();
            Whitelist = new WhiteList();
        }
    }
}