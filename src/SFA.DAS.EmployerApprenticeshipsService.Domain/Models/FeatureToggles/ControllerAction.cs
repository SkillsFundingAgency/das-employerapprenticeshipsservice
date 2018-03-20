using System;
using System.Collections.Generic;
using System.Configuration;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class ControllerAction
    {
        public ControllerAction(string qualifiedName)
        {
            var parts = qualifiedName.Split('.');
            if (parts.Length != 2)
            {
                throw new ArgumentException("The name must be in the form <controller-name>.<method-name>", nameof(qualifiedName));
            }

            SetValues(parts[0], parts[1]);
        }

        public ControllerAction(string controller, string action)
        {
            SetValues(controller, action);
        }

        private void SetValues(string controllerName, string actionName)
        {
            Controller = StripController(controllerName);
            Action = actionName;
            QualifiedName = $"{Controller}.{Action}";
        }

        public string Controller { get; private set; }

        public string Action { get; private set; }

        public string QualifiedName { get; private set; }

        public bool IsControllerLevel => Action == "*";

        public override bool Equals(object obj)
        {
            return obj is ControllerAction action &&
                   QualifiedName == action.QualifiedName;
        }

        public override int GetHashCode()
        {
            return -1980123662 + EqualityComparer<string>.Default.GetHashCode(QualifiedName);
        }

        public static string StripController(string controllerName)
        {
            const string controller = "Controller";

            if (controllerName.EndsWith(controller, StringComparison.InvariantCultureIgnoreCase))
            {
                return controllerName.Remove(controllerName.Length - controller.Length);
            }

            return controllerName;
        }
    }
}