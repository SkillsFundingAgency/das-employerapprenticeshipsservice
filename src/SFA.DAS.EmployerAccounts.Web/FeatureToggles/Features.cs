
namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public static class Features
    {
        public static IBooleanToggleValueProvider BooleanToggleValueProvider { get; set; }

        public static IFeatureToggle HomePage => new HomePageFeature();

        private class HomePageFeature : CloudConfigFeatureToggle
        {
            public HomePageFeature() : base(BooleanToggleValueProvider) { }
        }
    }
}