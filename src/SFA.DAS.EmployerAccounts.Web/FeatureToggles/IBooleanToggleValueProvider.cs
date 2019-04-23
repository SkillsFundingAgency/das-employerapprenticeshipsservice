namespace SFA.DAS.EmployerAccounts.Web.FeatureToggles
{
    public interface IBooleanToggleValueProvider
    {
        bool EvaluateBooleanToggleValue(IFeatureToggle toggle);
    }
}
