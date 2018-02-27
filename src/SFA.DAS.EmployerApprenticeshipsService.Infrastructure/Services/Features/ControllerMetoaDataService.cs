using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    public class ControllerMetoaDataService : IControllerMetaDataService
    {
        private readonly Lazy<Dictionary<Feature, ControllerAction[]>> _controllerActionsCache;

        public ControllerMetoaDataService()
        {
            _controllerActionsCache = new Lazy<Dictionary<Feature, ControllerAction[]>>();    
        }

        private static readonly ControllerAction[] EmptyControllerActions = new ControllerAction[0];

        public ControllerAction[] GetControllerMethodsLinkedToAFeature(Feature feature)
        {
            if (!_controllerActionsCache.Value.TryGetValue(feature, out var result))
            {
                result = EmptyControllerActions;
            }

            return result;
        }

        private Dictionary<Feature, ControllerAction[]> InitialiseControllerActionsCache()
        {
            var controllers = AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(Controller).IsAssignableFrom(type));

            return new Dictionary<Feature, ControllerAction[]>();
        }
    }
}