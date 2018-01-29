namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class FeatureToggle
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string[] WhiteList { get; set; }
    }
}