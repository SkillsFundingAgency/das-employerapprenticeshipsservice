
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

        public static IFeatureToggle EmulatedFundingJourney => new EmulatedFundingJourneyFeature();

        private class EmulatedFundingJourneyFeature : CloudConfigFeatureToggle
        {
            public EmulatedFundingJourneyFeature() : base(BooleanToggleValueProvider) { }
        }
    }
}