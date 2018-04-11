namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
	public class Feature
    {
        public bool Enabled { get; set; }
        public int? EnabledByAgreementVersion { get; set; }
        public FeatureType FeatureType { get; set; }
        public string Name { get; set; }
        public string[] Whitelist { get; set; }
    }
}