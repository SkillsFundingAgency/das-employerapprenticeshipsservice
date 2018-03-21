using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.Features;

namespace SFA.DAS.EAS.Web.Helpers
{
    public class ControllerMetaDataService : IControllerMetaDataService
    {
        private readonly Lazy<Dictionary<FeatureType, ControllerAction[]>> _controllerActionsCache;

        private static readonly ControllerAction[] EmptyControllerActions = new ControllerAction[0];

        private readonly Assembly _controllerAssembly;

        public ControllerMetaDataService(Assembly controllerAssembly = null)
        {
            _controllerActionsCache = new Lazy<Dictionary<FeatureType, ControllerAction[]>>(InitialiseControllerActionsCache);
            _controllerAssembly = controllerAssembly ?? this.GetType().Assembly;
        }

        public ControllerAction[] GetControllerMethodsLinkedToAFeature(FeatureType featureType)
        {
            if (!_controllerActionsCache.Value.TryGetValue(featureType, out var result))
            {
                result = EmptyControllerActions;
            }

            return result;
        }

        private Dictionary<FeatureType, ControllerAction[]> InitialiseControllerActionsCache()
        {
            var allControllers = GetAllControllers();

            var controllerActionsLinkedToFeature = GetControllerGlobalActions(allControllers)
                .Union(GetControllerMethodActions(allControllers))
                .GroupBy(controllerActions => controllerActions.Item1.RequiresAccessToFeatureType, controllerActions => controllerActions.Item2);


            var result = new Dictionary<FeatureType, ControllerAction[]>();

            foreach (var controllerAction in controllerActionsLinkedToFeature)
            {
                var deduppedControllerActions = GetControllerActionsForFeature(controllerAction.Key, controllerAction);
                result.Add(controllerAction.Key, deduppedControllerActions.ToArray());
            }

            return result;
        }

        private IEnumerable<ControllerAction> GetControllerActionsForFeature(FeatureType featureType, IEnumerable<ControllerAction> controllerActions)
        {
            var actionsByController = controllerActions.GroupBy(ca => ca.Controller);

            foreach (var controller in actionsByController)
            {
                var controllerAction = controller.FirstOrDefault(c => c.IsControllerLevel);
                if (controllerAction != null)
                {
                    yield return controllerAction;
                }
                else
                {
                    foreach (var action in controller.Distinct())
                    {
                        yield return action;
                    }
                }
            }
        }

        private Type[] GetAllControllers()
        {
            // This class must be in the Web app to find the controllers.
            return _controllerAssembly
                            .GetTypes()
                            .Where(type => typeof(Controller).IsAssignableFrom(type))
                            .ToArray();
        }

        private IEnumerable<(OperationFilterAttribute, ControllerAction)> GetControllerGlobalActions(Type[] controllers)
        {
            return controllers.Select(controller =>
                (
                    controller.GetCustomAttribute<OperationFilterAttribute>(false),
                    new ControllerAction(controller.Name, "*")
                ))
                .Where(controller => controller.Item1 != null && controller.Item1.RequiresAccessToFeatureType != FeatureType.NotSpecified);
        }

        private IEnumerable<(OperationFilterAttribute, ControllerAction)> GetControllerMethodActions(Type[] controllers)
        {
            return controllers
                .SelectMany(controller => controller
                    .GetMethods()
                    .Select(method => (
                        method.GetCustomAttribute<OperationFilterAttribute>(),
                        new ControllerAction(controller.Name, method.Name)))
                    .Where(ca => ca.Item1 != null && ca.Item1.RequiresAccessToFeatureType != FeatureType.NotSpecified));
        }
    }
}