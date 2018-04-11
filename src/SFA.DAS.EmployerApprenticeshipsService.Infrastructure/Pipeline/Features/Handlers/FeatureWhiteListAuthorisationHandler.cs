using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{
    public class FeatureWhitelistAuthorisationHandler : IAuthorizationHandler
    {
        public Task<bool> CanAccessAsync(IAuthorizationContext authorizationContext)
        {
            return IsFeatureEnabledForContextAsync(authorizationContext);
        }

        public Task<bool> IsFeatureEnabledForContextAsync(IAuthorizationContext authorisationContext)
        {
            var feature = authorisationContext.CurrentFeature;

            if (feature?.Whitelist == null)
            {
                return FeatureHandlerResults.FeatureEnabledTask;
            }

            if (string.IsNullOrWhiteSpace(authorisationContext.UserContext?.Email))
            {
                return FeatureHandlerResults.FeatureDisabledTask;
            }

            if (feature.Whitelist.Any(email => Regex.IsMatch(authorisationContext.UserContext.Email, email, RegexOptions.IgnoreCase)))
            {
                return FeatureHandlerResults.FeatureEnabledTask;
            }

            return FeatureHandlerResults.FeatureDisabledTask;
        }
    }
}