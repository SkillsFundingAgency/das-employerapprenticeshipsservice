using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureEnabledAuthorisationHandler : IAuthorizationHandler
    {
        private readonly ILog _logger;
        private static readonly Type Type = typeof(FeatureEnabledAuthorisationHandler);

        public FeatureEnabledAuthorisationHandler(ILog logger)
        {
            _logger = logger;
        }

        public Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            _logger.Debug($"Started running '{Type.Name}' for feature '{feature.FeatureType}'");

            var result = feature.Enabled
                ? Task.FromResult(AuthorizationResult.Ok)
                : Task.FromResult(AuthorizationResult.FeatureDisabled);

            _logger.Debug($"Finished running '{Type.Name}' for feature '{feature.FeatureType}' with result '{result}'");

            return result;
        }
    }
}