namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IFeatureToggleService
    {
        bool IsFeatureEnabled(string controllerName, string actionName, string userEmail);
    }
}
