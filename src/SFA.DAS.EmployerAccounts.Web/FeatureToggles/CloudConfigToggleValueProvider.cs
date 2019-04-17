using FeatureToggle;
using Microsoft.Azure;
using SFA.DAS.EmployerAccounts.Configuration;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public class CloudConfigToggleValueProvider : IBooleanToggleValueProvider
    {
        private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;
        public CloudConfigToggleValueProvider(EmployerAccountsConfiguration employerAccountsConfiguration)
        {
            _employerAccountsConfiguration = employerAccountsConfiguration;
        }
        public bool EvaluateBooleanToggleValue(IFeatureToggle toggle)
        {
            var featureToggle = _employerAccountsConfiguration.FeatureToggles.FirstOrDefault(i => i.Name.Equals(toggle.GetType().Name));

            if (featureToggle == null)
            {
                var setting = CloudConfigurationManager.GetSetting($"FeatureToggle.{toggle.GetType().Name}");

                return !string.IsNullOrEmpty(setting) && bool.Parse(setting);
            }

            return featureToggle.Value;
        }
    }
}