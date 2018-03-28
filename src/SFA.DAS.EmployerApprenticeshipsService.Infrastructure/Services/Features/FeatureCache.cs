using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class FeatureCache : IFeatureCache
    {
        private readonly HashSet<string> _toggledControllers;
        private readonly Dictionary<string, ControllerActionCacheItem> _controllerActions;

        public FeatureCache(
            Feature[] toggledFeatures,
            IControllerMetaDataService controllerMetaDataService)
        {
            var allFeatures = BuildAllFeatures(toggledFeatures).ToArray();

            _toggledControllers = BuildControllerNamesCache(allFeatures, controllerMetaDataService);
            _controllerActions = BuildControllerActionsCache(allFeatures, controllerMetaDataService);
        }

        private IEnumerable<Feature> BuildAllFeatures(Feature[] toggledFeatures)
        {
            foreach (var featureType in Enum.GetValues(typeof(FeatureType)).Cast<FeatureType>())
            {
                var feature = toggledFeatures?.SingleOrDefault(toggledFeature => toggledFeature.FeatureType == featureType);
                yield return feature ?? new Feature { FeatureType = featureType };
            }
        }

        public bool IsControllerSubjectToFeature(string controllerName)
        {
            return _toggledControllers.Contains(ControllerAction.StripController(controllerName));
        }

        public bool TryGetControllerActionSubjectToFeature(OperationContext context, out ControllerActionCacheItem controllerAction)
        {
            controllerAction = null;

            var controllerName = ControllerAction.StripController(context.Controller);

            if (!IsControllerSubjectToFeature(controllerName))
            {
                return false;
            }

            if (_controllerActions.TryGetValue($"{controllerName}.{context.Action}", out controllerAction))
            {
                return true;
            }

            if (_controllerActions.TryGetValue($"{controllerName}.*", out controllerAction))
            {
                return true;
            }

            return false;
        }

        private HashSet<string> BuildControllerNamesCache(Feature[] features, IControllerMetaDataService controllerMetaDataService)
        {
            var result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            var controllerNames = features
                .SelectMany(feature =>
                    controllerMetaDataService.GetControllerMethodsLinkedToAFeature(feature.FeatureType))
                .Select(a => a.Controller)
                .Distinct();

            foreach (var controllerName in controllerNames)
            {
                result.Add(controllerName);
            }

            return result;
        }

        private Dictionary<string, ControllerActionCacheItem> BuildControllerActionsCache(
            Feature[] features, IControllerMetaDataService controllerMetaDataService)
        {
            var temp = new Dictionary<string, ControllerActionCacheItem>(StringComparer.InvariantCultureIgnoreCase);

            var allFeatureControlledOperations = features
                .SelectMany(feature =>
                    controllerMetaDataService.GetControllerMethodsLinkedToAFeature(feature.FeatureType)
                        .Select(controllerAction => new ControllerActionCacheItem(controllerAction, feature)));

            foreach (var controllerActionCacheItem in allFeatureControlledOperations)
            {
                temp.Add(controllerActionCacheItem.QualifiedName, controllerActionCacheItem);
            }

            return temp;
        }
    }
}