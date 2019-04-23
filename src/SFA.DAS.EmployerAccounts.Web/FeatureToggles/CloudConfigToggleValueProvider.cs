using Microsoft.Azure;
using SFA.DAS.Authorization;
using System;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public class CloudConfigToggleValueProvider : IBooleanToggleValueProvider
    {
        private readonly FeaturesConfiguration _featuresConfiguration;
        public CloudConfigToggleValueProvider(FeaturesConfiguration featuresConfiguration)
        {
            _featuresConfiguration = featuresConfiguration;
        }
        public bool EvaluateBooleanToggleValue(IFeatureToggle toggle)
        {
            var featureToggle = _featuresConfiguration.Data?.FirstOrDefault(f => Enum.GetName(typeof(FeatureType), f.FeatureType).Equals(toggle.GetType().Name));

            if (featureToggle == null)
            {
                var setting = CloudConfigurationManager.GetSetting($"FeatureToggle.{toggle.GetType().Name}");

                return !string.IsNullOrEmpty(setting) && bool.Parse(setting);
            }

            return featureToggle.Enabled;
        }
    }
}