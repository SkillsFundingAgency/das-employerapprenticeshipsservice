using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;

namespace SFA.DAS.EAS.Web.Helpers
{
    public class ControllerMetaDataService : IControllerMetaDataService
    {
        private readonly Lazy<Dictionary<Feature, ControllerAction[]>> _controllerActionsCache;

        private static readonly ControllerAction[] EmptyControllerActions = new ControllerAction[0];

        public ControllerMetaDataService()
        {
            _controllerActionsCache = new Lazy<Dictionary<Feature, ControllerAction[]>>(InitialiseControllerActionsCache);    
        }

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
            var allControllers = GetAllControllers();

            var controllerActionsLinkedToFeature = GetControllerGlobalActions(allControllers)
                .Union(GetControllerMethodActions(allControllers))
                .GroupBy(controllerActions => controllerActions.Item1.RequiresAccessToFeature, controllerActions => controllerActions.Item2);


            var result = new Dictionary<Feature, ControllerAction[]>();

            foreach (var controllerAction in controllerActionsLinkedToFeature)
            {
                var deduppedControllerActions = GetControllerActionsForFeature(controllerAction.Key, controllerAction);
                result.Add(controllerAction.Key, deduppedControllerActions.ToArray());
            }

            return result;
        }

        private IEnumerable<ControllerAction> GetControllerActionsForFeature(Feature feature, IEnumerable<ControllerAction> controllerActions)
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
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(Controller).IsAssignableFrom(type))
                .ToArray();
        }

        private IEnumerable<(OperationFilterAttribute, ControllerAction)> GetControllerGlobalActions(Type[] controllers)
        {
            var controllersWithFeatureLink = controllers.Select(controller => new
                {
                    Controller = controller,
                    OperationFilterAttributes = controller.GetCustomAttributes<OperationFilterAttribute>(false).Where(ofa => ofa.RequiresAccessToFeature != Feature.NotSpecified).ToArray()
                })
                .Where(controller => controller.OperationFilterAttributes.Length > 0);

            foreach (var controller in controllersWithFeatureLink)
            {
                foreach (var attribute in controller.OperationFilterAttributes)
                {
                    yield return (attribute, new ControllerAction(controller.Controller.Name, "*"));
                }
            }
        }

        private IEnumerable<(OperationFilterAttribute, ControllerAction)> GetControllerMethodActions(Type[] controllers)
        {
            var controllersAndMethods = controllers.Select(controller => new
                {
                    Controller = controller,
                    Methods = controller.GetMethods().Select(method => new
                    {
                        Method = method,
                        MethodAttributes = method.GetCustomAttributes<OperationFilterAttribute>(false).Where(ofa => ofa.RequiresAccessToFeature != Feature.NotSpecified).ToArray()
                    }).Where(method => method.MethodAttributes.Length > 0).ToArray()
                })
                .Where(controller => controller.Methods.Length > 0);


            foreach (var controllerMethods in controllersAndMethods)
            {
                foreach (var controllerMethod in controllerMethods.Methods)
                {
                    foreach (var methodAttribute in controllerMethod.MethodAttributes)
                    {
                        yield return (methodAttribute, new ControllerAction(controllerMethods.Controller.Name, controllerMethod.Method.Name));
                    }
                }
            }
        }
    }
}