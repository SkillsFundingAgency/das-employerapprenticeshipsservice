using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    public class FeatureToggleCache : IFeatureToggleCache
    {
        private readonly HashSet<string> _toggledControllers;
        private readonly Dictionary<string, ControllerActionCacheItem> _controllerActions;
        private readonly IControllerMetaDataService _controllerMetaDataService;

        public FeatureToggleCache(
            FeatureToggleCollection featureToggleCollection, 
            IControllerMetaDataService controllerMetaDataService)
        {
            _controllerMetaDataService = controllerMetaDataService;

            _toggledControllers = BuildControllerNamesCache(featureToggleCollection);
            _controllerActions = BuildControllerActionsCache(featureToggleCollection);
            IsAvailable = featureToggleCollection?.Features?.Count != null;
        }

        public bool IsAvailable { get; private set; }

        public bool IsControllerSubjectToFeatureToggle(string controllerName)
        {
            return _toggledControllers.Contains(ControllerAction.StripController(controllerName));
        }

        public bool TryGetControllerActionSubjectToToggle(string controllerName, string actionName, out ControllerActionCacheItem controllerAction)
        {
            controllerAction = null;

            controllerName = ControllerAction.StripController(controllerName);

            if (!IsControllerSubjectToFeatureToggle(controllerName))
            {
                return false;
            }

            return _controllerActions.TryGetValue($"{controllerName}.{actionName}", out controllerAction);
        }

        private HashSet<string> BuildControllerNamesCache(FeatureToggleCollection featureToggleCollection)
        {
            var result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            if (featureToggleCollection?.Features != null)
            {
                var controllerNames = featureToggleCollection
                                        .Features
                                        .SelectMany(feature => _controllerMetaDataService.GetControllerMethodsLinkedToAFeature(feature.Feature))
                                        .Select(a => a.Controller)
                                        .Distinct();

                foreach (var controllerName in controllerNames)
                {
                    result.Add(controllerName);
                }
            }

            return result;
        }

        private Dictionary<string, ControllerActionCacheItem> BuildControllerActionsCache(FeatureToggleCollection featureToggleCollection)
        {
            var temp = new Dictionary<string, ControllerActionCacheItem>(StringComparer.InvariantCultureIgnoreCase);

            if (featureToggleCollection?.Features != null)
            {
                foreach (var feature in featureToggleCollection.Features)
                {
                    var controllerActionsForThisFeature =
                        _controllerMetaDataService.GetControllerMethodsLinkedToAFeature(feature.Feature);

                    foreach (var action in controllerActionsForThisFeature)
                    {
                        var cachedItem = Ensure(temp, action);
                        cachedItem.WhiteLists.Add(feature.Whitelist);
                    }
                }
            }

            return temp;
        }

        private ControllerActionCacheItem Ensure(Dictionary<string, ControllerActionCacheItem> cache, ControllerAction action)
        {
            if (!cache.TryGetValue(action.QualifiedName, out var result))
            {
                result = new ControllerActionCacheItem(action.Controller, action.Action);
                cache.Add(action.QualifiedName, result);
            }

            return result;
        }
    }
}