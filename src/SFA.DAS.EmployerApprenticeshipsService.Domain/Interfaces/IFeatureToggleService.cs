using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IFeatureToggleService
    {
        bool IsFeatureEnabled(string controllerName, string actionName, IAuthorizationContext authorizationContext);
    }
}
