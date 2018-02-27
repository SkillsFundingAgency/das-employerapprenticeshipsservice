using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class Feature
    {
        public FeatureType FeatureType { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public string[] WhiteList { get; set; }

        /// <summary>
        ///     The version of the agreement that introduced this feature. Signing this or any subsequent
        ///     agreement entitles access to this feature.
        /// </summary>
        public decimal EnabledByAgreementVersion { get; set; }
    }
}