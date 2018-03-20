using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    /// <summary>
    ///     Responsible for providing a list of contgroller actions that are linked to a given feature.
    /// </summary>
    public interface IControllerMetaDataService
    {
        ControllerAction[] GetControllerMethodsLinkedToAFeature(Feature feature);
    };
}