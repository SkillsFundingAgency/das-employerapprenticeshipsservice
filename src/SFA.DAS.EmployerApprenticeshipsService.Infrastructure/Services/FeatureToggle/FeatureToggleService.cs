using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using SFA.DAS.NLog.Logger;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Sections
{
    public class FeatureToggleService : AzureServiceBase<FeatureToggleConfiguration>, IFeatureToggleService
    {
        private readonly ICacheProvider _cacheProvider;
        public sealed override ILog Logger { get; set; }

        public static TimeSpan DefaultCacheTime { get; } = new TimeSpan(0, 0, 30, 0);
        public static TimeSpan ShortLivedCacheTime { get; } = new TimeSpan(0, 0, 1, 0);
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.Features";

        public FeatureToggleService(ICacheProvider cacheProvider, ILog logger)
        {
            _cacheProvider = cacheProvider;
            Logger = logger;
        }

        public async Task<bool> IsFeatureEnabled(OperationContext context)
        {
            return await Task.Run(() =>
            {
                var cacheConfig = GetConfiguration();

                bool isFeatureEnabled;

                if (cacheConfig.TryGetControllerActionSubjectToToggle(context.Controller, context.Action, out ControllerActionCacheItem controllerAction))
                {
                    isFeatureEnabled = IsFeatureEnabled(context, controllerAction);
                }
                else
                {
                    isFeatureEnabled = true;
                }

                Logger.Info($"Is feature enabled check for controllerName '{context.Controller}', " +
                            $"actionName '{context.Action}' and userId '{context.MembershipContext?.UserId}' " +
                            $"is '{isFeatureEnabled}'.");


                return isFeatureEnabled;
            }).ConfigureAwait(false);
        }

        private static bool IsFeatureEnabled(OperationContext context, ControllerActionCacheItem controllerAction)
        {
            if(string.IsNullOrWhiteSpace(context.MembershipContext?.UserEmail))
                return false;

            return controllerAction.WhiteLists.Any(whiteList => IsWhiteListed(context, whiteList));
        }

        private static bool IsWhiteListed(OperationContext context, WhiteList whiteList)
        {
            return whiteList.Emails.Any(email => Regex.IsMatch(context.MembershipContext.UserEmail, email, RegexOptions.IgnoreCase));
        }

        private IFeatureToggleCache GetConfiguration()
        {
            var cachedConfig = _cacheProvider.Get<IFeatureToggleCache>(nameof(FeatureToggleCache));
            if (cachedConfig == null)
            {
                var config = GetDataFromTableStorage();

                cachedConfig = new FeatureToggleCache(config.Data);
                _cacheProvider.Set(nameof(FeatureToggleCache), cachedConfig,
                    cachedConfig.IsAvailable ? DefaultCacheTime : ShortLivedCacheTime);
            }

            return cachedConfig;
        }
    }
}
