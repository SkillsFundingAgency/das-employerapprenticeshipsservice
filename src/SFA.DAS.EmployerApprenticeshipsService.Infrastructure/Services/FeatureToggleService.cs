using System;
using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
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

        public virtual bool IsFeatureEnabled(string controllerName, string actionName, IAuthorizationContext authorizationContext)
        {
            var config = GetConfiguration();
            var isFeatureEnabled = true;

            if (config?.Data != null)
            {
                var controllerToggles = config.Data.Where(c => c.Controller == controllerName).ToList();

                if (controllerToggles.Any())
                {
                    var actionToggle = controllerToggles.Where(t => t.Action == actionName || t.Action == "*").OrderByDescending(t => t.Action).FirstOrDefault();

                    if (actionToggle != null)
                    {
                        var whitelistToggle = authorizationContext.UserContext != null && actionToggle.Whitelist.Any(p => Regex.IsMatch(authorizationContext.UserContext.Email, p, RegexOptions.IgnoreCase));

                        if (!whitelistToggle)
                        {
                            isFeatureEnabled = false;
                        }
                    }
                }
            }
            
            Logger.Info($"Is feature enabled check for controllerName '{controllerName}', actionName '{actionName}' and userId '{authorizationContext.UserContext?.Id}' is '{isFeatureEnabled}'.");

            return isFeatureEnabled;
        }

        private FeatureToggleConfiguration GetConfiguration()
        {
            var config = _cacheProvider.Get<FeatureToggleConfiguration>(nameof(FeatureToggleConfiguration));

            if (config == null)
            {
                config = GetDataFromTableStorage();

                if (config?.Data != null && config.Data.Any())
                {
                    _cacheProvider.Set(nameof(FeatureToggleConfiguration), config, new TimeSpan(0, 30, 0));
                }
            }

            return config;
        }
    }
}
