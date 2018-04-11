using System;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class ControllerActionCacheItem
    {
        private readonly int _hashvalue;
        private readonly ControllerAction _controllerAction;

        public ControllerActionCacheItem(ControllerAction controllerAction, Feature feature)
        {
            _controllerAction = controllerAction;
            Feature = feature;
            _hashvalue = GetHash(Controller, Action);
        }

        public string Controller => _controllerAction.Controller;

        public string Action => _controllerAction.Action;

        public string QualifiedName => _controllerAction.QualifiedName;

        public override int GetHashCode()
        {
            return _hashvalue;
        }

        public override bool Equals(object obj)
        {
            return (obj is ControllerActionCacheItem action) &&
                   string.Equals(action.Controller, Controller, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(action.Action, Action, StringComparison.InvariantCultureIgnoreCase);
        }

        public Feature Feature { get; }

        public string[] Whitelist => Feature.Whitelist;

        public FeatureType FeatureType => Feature.FeatureType;

        public int EnabledByAgreementVersion => Feature.EnabledByAgreementVersion;

        private int GetHash(string controller, string action)
        {
            return (controller?.ToUpperInvariant().GetHashCode() ?? 0) << 16 & (action?.ToUpperInvariant().GetHashCode() ?? 0);
        }
    }
}