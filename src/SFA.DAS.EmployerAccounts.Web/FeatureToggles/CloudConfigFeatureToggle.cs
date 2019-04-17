using FeatureToggle;

namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public class CloudConfigFeatureToggle : SimpleFeatureToggle
    {
        public CloudConfigFeatureToggle(IBooleanToggleValueProvider booleanToggleValueProvider )
        {
            base.ToggleValueProvider = booleanToggleValueProvider;
        }
        public sealed override IBooleanToggleValueProvider ToggleValueProvider { get => base.ToggleValueProvider;}
    }

}