using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    public class FeatureToggleService : IFeatureToggleService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IFeatureToggleCacheFactory _featureToggleCacheFactory;

        public static TimeSpan DefaultCacheTime { get; } = new TimeSpan(0, 0, 30, 0);
        public static TimeSpan ShortLivedCacheTime { get; } = new TimeSpan(0, 0, 1, 0);
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.Features";
        public FeatureToggleService(ICacheProvider cacheProvider, ILog logger,
            IFeatureToggleCacheFactory featureToggleCacheFactory)
            _featureToggleCacheFactory = featureToggleCacheFactory;
        public async Task<bool> IsFeatureEnabled(OperationContext context)
        {
            return await Task.Run(() =>
            {
                var cacheConfig = GetConfiguration();

                bool isFeatureEnabled;

                if (cacheConfig.TryGetControllerActionSubjectToToggle(context.Controller, context.Action,
                    out ControllerActionCacheItem controllerAction))
                {
                    isFeatureEnabled = IsFeatureEnabled(context, controllerAction);
                }
                else
                {
                    isFeatureEnabled = true;
                }
                Logger.Info($"Is feature enabled check for controllerName '{context.Controller}', " +
                            $"actionName '{context.Action}' and userId '{context.AuthorisationContext?.UserContext?.Id}' " +
                            $"is '{isFeatureEnabled}'.");

                return isFeatureEnabled;
            }).ConfigureAwait(false);
        }

        private static bool IsFeatureEnabled(OperationContext context, ControllerActionCacheItem controllerAction)
        {
            if (string.IsNullOrWhiteSpace(context.AuthorisationContext?.UserContext?.Email))
                return false;

            return controllerAction.WhiteLists.Any(whiteList => IsWhiteListed(context, whiteList));
        }

        private static bool IsWhiteListed(OperationContext context, WhiteList whiteList)
        {
            return string.IsNullOrWhiteSpace(context.AuthorisationContext?.UserContext?.Email) || 
                   whiteList.Emails.Any(email =>
                                Regex.IsMatch(context.AuthorisationContext?.UserContext?.Email, email, RegexOptions.IgnoreCase));
        }

        private IFeatureToggleCache GetConfiguration()
        {
            var cachedConfig = _cacheProvider.Get<IFeatureToggleCache>(nameof(FeatureToggleCache));
            {
                Controller = controllerName,
                Action = actionName,
                MembershipContext = membershipContext
            };

                cachedConfig = _featureToggleCacheFactory.Create(config.Data);
                _cacheProvider.Set(nameof(FeatureToggleCache), cachedConfig,
                    cachedConfig.IsAvailable ? DefaultCacheTime : ShortLivedCacheTime);

            return _featurePipeline.ProcessAsync(request).Result;
        }
    }

    public interface IFeatureToggleCacheFactory
    {
        IFeatureToggleCache Create(FeatureToggleCollection features);
    }

    public class FeatureToggleCacheFactory : IFeatureToggleCacheFactory
    {
        private readonly IControllerMetaDataService _controllerMetaDataService;

        public FeatureToggleCacheFactory(IControllerMetaDataService controllerMetaDataService, IControllerMetaDataService controllerMetaDataService1)
        {
            _controllerMetaDataService = controllerMetaDataService1;
        }

        public IFeatureToggleCache Create(FeatureToggleCollection features)
        {
            return new FeatureToggleCache(features, _controllerMetaDataService);
        }
    }
}
