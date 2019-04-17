using FeatureToggle;

namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{

    public class HomePageFeature : CloudConfigFeatureToggle
    {
        public HomePageFeature(IBooleanToggleValueProvider booleanToggleValueProvider) : base(booleanToggleValueProvider)
        {            
        }
    }
}