using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class FeatureWhitelistAuthorizationHandler : IAuthorizationHandler
    {
        public Task<bool> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            if (feature?.Whitelist == null)
            {
                return FeatureHandlerResults.FeatureEnabledTask;
            }

            if (string.IsNullOrWhiteSpace(authorizationContext.UserContext?.Email))
            {
                return FeatureHandlerResults.FeatureDisabledTask;
            }

            if (feature.Whitelist.Any(email => Regex.IsMatch(authorizationContext.UserContext.Email, email, RegexOptions.IgnoreCase)))
            {
                return FeatureHandlerResults.FeatureEnabledTask;
            }

            return FeatureHandlerResults.FeatureDisabledTask;
        }
    }
}