using System;
using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
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


        public static TimeSpan DefaultCacheTime { get; } = new TimeSpan(0, 0, 30, 0);
        public static TimeSpan ShortLivedCacheTime { get; } = new TimeSpan(0, 0, 1, 0);

        public virtual bool IsFeatureEnabled(string controllerName, string actionName, IMembershipContext membershipContext)
        {
            var cacheConfig = GetConfiguration();

            bool isFeatureEnabled;

            if (cacheConfig.TryGetControllerActionSubjectToToggle(controllerName, actionName, out ControllerActionCacheItem controllerAction))
            {
                isFeatureEnabled = membershipContext?.UserEmail != null &&
                    controllerAction.WhiteLists.Any(whiteList => whiteList.Emails != null && whiteList.Emails.Any(email => Regex.IsMatch(membershipContext.UserEmail, email, RegexOptions.IgnoreCase)));
            }
            else
            { 
                isFeatureEnabled = true;
            }

            Logger.Info($"Is feature enabled check for controllerName '{controllerName}', actionName '{actionName}' and userId '{membershipContext?.UserId}' is '{isFeatureEnabled}'.");

            return isFeatureEnabled;
        }

        private FeatureToggleCache GetConfiguration()
        {
            var cachedConfig = _cacheProvider.Get<FeatureToggleCache>(nameof(FeatureToggleCache));
            if (cachedConfig == null)
            {
                var config = GetDataFromTableStorage();

                cachedConfig = new FeatureToggleCache(config.Data);
                _cacheProvider.Set(nameof(FeatureToggleCache), cachedConfig, cachedConfig.IsAvailable ? DefaultCacheTime : ShortLivedCacheTime);
            }

            return cachedConfig;
        }
    }
}
