using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureWhitelistAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILog _logger;
        private static readonly Type Type = typeof(FeatureWhitelistAuthorizationHandler);

        public FeatureWhitelistAuthorizationHandler(ILog logger)
        {
            _logger = logger;
        }

        public Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            Task<AuthorizationResult> result;

            _logger.Debug($"Started running '{Type.Name}' for feature '{feature.FeatureType}'");
            
            if (feature.Whitelist == null)
            {
                result = Task.FromResult(AuthorizationResult.Ok);
            }
            else if (string.IsNullOrWhiteSpace(authorizationContext.UserContext?.Email))
            {
                result = Task.FromResult(AuthorizationResult.FeatureUserNotWhitelisted);
            }
            else if (feature.Whitelist.Any(email => Regex.IsMatch(authorizationContext.UserContext.Email, email, RegexOptions.IgnoreCase)))
            {
                result = Task.FromResult(AuthorizationResult.Ok);
            }
            else
            {
                result = Task.FromResult(AuthorizationResult.FeatureUserNotWhitelisted);
            }

            _logger.Debug($"Finished running '{Type.Name}' for feature '{feature.FeatureType}' with result '{result}'");

            return result;
        }
    }
}