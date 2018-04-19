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
        public Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            if (feature?.Whitelist == null)
            {
                return Task.FromResult(AuthorizationResult.Ok);
            }

            if (string.IsNullOrWhiteSpace(authorizationContext.UserContext?.Email))
            {
                return Task.FromResult(AuthorizationResult.FeatureUserNotWhitelisted);
            }

            if (feature.Whitelist.Any(email => Regex.IsMatch(authorizationContext.UserContext.Email, email, RegexOptions.IgnoreCase)))
            {
                return Task.FromResult(AuthorizationResult.Ok);
            }

            return Task.FromResult(AuthorizationResult.FeatureUserNotWhitelisted);
        }
    }
}