using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using SFA.DAS.NLog.Logger;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Sections
{
    public class WhitelistPipelineSection :
        AzureServiceBase<FeatureToggleConfiguration>,
        IPipelineSection<FeatureToggleRequest, bool>
    {
        private readonly ICacheProvider _cacheProvider;
        public sealed override ILog Logger { get; set; }

        public static TimeSpan DefaultCacheTime { get; } = new TimeSpan(0, 0, 30, 0);
        public static TimeSpan ShortLivedCacheTime { get; } = new TimeSpan(0, 0, 1, 0);

        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.Features";

        public uint Priority => 1;

        public WhitelistPipelineSection(ICacheProvider cacheProvider, ILog logger)
        {
            _cacheProvider = cacheProvider;
            Logger = logger;
        }

        public async Task<bool> ProcessAsync(FeatureToggleRequest request)
        {
            return await Task.Run(() =>
            {
                var cacheConfig = GetConfiguration();

                bool isFeatureEnabled;

                if (cacheConfig.TryGetControllerActionSubjectToToggle(
                    request.Controller,
                    request.Action,
                    out ControllerActionCacheItem controllerAction))
                {
                    isFeatureEnabled = request.MembershipContext?.UserEmail != null &&
                                       controllerAction.WhiteLists.Any(whiteList =>
                                           whiteList.Emails != null && whiteList.Emails.Any(email =>
                                               Regex.IsMatch(request.MembershipContext.UserEmail, email,
                                                   RegexOptions.IgnoreCase)));
                }
                else
                {
                    isFeatureEnabled = true;
                }

                Logger.Info($"Is feature enabled check for controllerName '{request.Controller}', " +
                            $"actionName '{request.Action}' and userId '{request.MembershipContext?.UserId}' " +
                            $"is '{isFeatureEnabled}'.");


                return isFeatureEnabled;
            }).ConfigureAwait(false);
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
