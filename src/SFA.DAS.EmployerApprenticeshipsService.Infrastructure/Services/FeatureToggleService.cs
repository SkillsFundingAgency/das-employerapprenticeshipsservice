using System;
using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class FeatureToggleService : AzureServiceBase<FeatureToggleConfiguration>, IFeatureToggleService
    {
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.Features";
        public sealed override ILog Logger { get; set; }

        private readonly ICacheProvider _cacheProvider;

        public FeatureToggleService(ICacheProvider cacheProvider, ILog logger)
        {
            _cacheProvider = cacheProvider;
            Logger = logger;
        }

        public virtual bool IsFeatureEnabled(string controllerName, string actionName, string userEmail)
        {
            var features = GetFeatures();

            if (features?.Data == null)
            {
                return true;
            }
            
            var controllerToggles = features.Data
                .Where(c => c.Controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            if (!controllerToggles.Any())
            {
                return true;
            }
            
            var actionToggle = controllerToggles
                .Where(t => t.Action.Equals(actionName, StringComparison.CurrentCultureIgnoreCase) || t.Action == "*")
                .OrderByDescending(t => t.Action) // Should put action = * last as specific action toggle should win
                .FirstOrDefault();

            if (actionToggle == null)
            {
                return true;
            }

            if (actionToggle.WhiteList == null)
            {
                return false;
            }

            return actionToggle.WhiteList.Any(p => Regex.IsMatch(userEmail, p, RegexOptions.IgnoreCase));
        }

        private FeatureToggleConfiguration GetFeatures()
        {
            var features = _cacheProvider.Get<FeatureToggleConfiguration>(nameof(FeatureToggleConfiguration));

            if (features == null)
            {
                features = GetDataFromStorage();

                if (features?.Data != null && features.Data.Any())
                {
                    _cacheProvider.Set(nameof(FeatureToggleConfiguration), features, new TimeSpan(0, 30, 0));
                }
            }

            return features;
        }
    }
}
