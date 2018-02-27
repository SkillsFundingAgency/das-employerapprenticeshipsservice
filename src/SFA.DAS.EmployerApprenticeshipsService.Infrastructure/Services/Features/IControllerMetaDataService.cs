using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    /// <summary>
    ///     Responsible for providing a list of controller actions that are linked to a given feature.
    /// </summary>
    public interface IControllerMetaDataService
    {
        ControllerAction[] GetControllerMethodsLinkedToAFeature(FeatureType featureType);
    };
}