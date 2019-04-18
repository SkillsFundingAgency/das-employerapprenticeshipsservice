
namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public abstract class CloudConfigFeatureToggle : IFeatureToggle
    {
        private readonly IBooleanToggleValueProvider _booleanToggleValueProvider;
        public bool Enabled => _booleanToggleValueProvider.EvaluateBooleanToggleValue(this);

        protected CloudConfigFeatureToggle(IBooleanToggleValueProvider booleanToggleValueProvider)
        {
            _booleanToggleValueProvider = booleanToggleValueProvider;
        }
    }
}