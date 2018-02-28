using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    public class ControllerActionCacheItem
    {
        private readonly int _hashvalue;

        public ControllerActionCacheItem(string controller, string action)
        {
            Controller = controller;
            Action = action;
            WhiteLists = new List<WhiteList>();
            _hashvalue = GetHash(Controller, Action);
        }

        public string Controller { get; }

        public string Action { get; }

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

        public List<WhiteList> WhiteLists { get; }

        private int GetHash(string controller, string action)
        {
            return (controller?.ToUpperInvariant().GetHashCode() ?? 0) << 16 & (action?.ToUpperInvariant().GetHashCode() ?? 0);
        }
    }
}