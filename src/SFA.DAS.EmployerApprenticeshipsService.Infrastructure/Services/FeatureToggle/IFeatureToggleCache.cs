using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    public interface IFeatureToggleCache
    {
        bool IsAvailable { get; }
        bool IsControllerSubjectToFeatureToggle(string controllerName);
        bool TryGetControllerActionSubjectToToggle(string controllerName, string actionName, out ControllerActionCacheItem controllerAction);
    }
}