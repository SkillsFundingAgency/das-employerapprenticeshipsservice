using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public interface IFeatureCache
    {
        bool IsControllerSubjectToFeature(string controllerName);
        bool TryGetControllerActionSubjectToFeature(string controllerName, string operationName, out ControllerActionCacheItem controllerAction);
    }
}