namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class Feature
    {
        public bool Enabled { get; set; }

        /// <summary>
        ///     The version of the agreement that introduced this feature. Signing this or any subsequent
        ///     agreement entitles access to this feature.
        /// </summary>
        public decimal EnabledByAgreementVersion { get; set; }

        public FeatureType FeatureType { get; set; }
        public string Name { get; set; }
        public string[] WhiteList { get; set; }
    }
}