using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers
{

    public class FeatureWhiteListAuthorisationHandler : IOperationAuthorisationHandler
    {
        public Task<bool> CanAccessAsync(IAuthorizationContext authorisationContext)
        {
            return IsFeatureEnabledForContextAsync(authorisationContext);
        }

        public Task<bool> IsFeatureEnabledForContextAsync(IAuthorizationContext authorisationContext)
        {
            var feature = authorisationContext.CurrentFeature;

            if (feature?.WhiteList == null)
            {
                return FeatureHandlerResults.FeatureEnabledTask;
            }

            if (string.IsNullOrWhiteSpace(authorisationContext?.UserContext?.Email))
            {
                return FeatureHandlerResults.FeatureDisabledTask;
            }

            if (feature.WhiteList.Any(email =>
                Regex.IsMatch(authorisationContext.UserContext.Email, email, RegexOptions.IgnoreCase)))
            {
                return FeatureHandlerResults.FeatureEnabledTask;
            }

            return FeatureHandlerResults.FeatureDisabledTask;
        }
    }
}